import os
import tensorflow as tf
from tensorflow.keras.models import Sequential, load_model
from tensorflow.keras.layers import Dense, Flatten, Dropout
from tensorflow.keras.optimizers import Adam
from keras_tuner import RandomSearch
import csv

# Paths
train_data_path = "/home/quad/dataset_6000/train"
val_data_path = "/home/quad/dataset_6000/val"
test_data_path = "/home/quad/dataset_6000/test"
previous_model_path = "/home/quad/retrain_dataset_6000/models/best_model_epoch_4.keras"  # Path to your pre-trained model
fine_tuned_models_dir = "/home/quad/retrain_dataset_6000/models/fine_tuned_models"
summary_log_file = "/home/quad/retrain_dataset_6000/models/fine_tuned_models/tuning_summary.csv"

# Ensure directory exists for saving fine-tuned models
os.makedirs(fine_tuned_models_dir, exist_ok=True)

# Data generators
train_datagen = tf.keras.preprocessing.image.ImageDataGenerator(rescale=1.0 / 255)
val_datagen = tf.keras.preprocessing.image.ImageDataGenerator(rescale=1.0 / 255)
test_datagen = tf.keras.preprocessing.image.ImageDataGenerator(rescale=1.0 / 255)

train_generator = train_datagen.flow_from_directory(
    train_data_path, target_size=(224, 224), batch_size=32, class_mode="categorical"
)

val_generator = val_datagen.flow_from_directory(
    val_data_path, target_size=(224, 224), batch_size=32, class_mode="categorical"
)

test_generator = test_datagen.flow_from_directory(
    test_data_path, target_size=(224, 224), batch_size=32, class_mode="categorical", shuffle=False
)


# Hypermodel: Define a function for building models
def build_model(hp):
    # Load the pre-trained model
    base_model = load_model(previous_model_path)
    base_model.trainable = True  # Unfreeze all layers for fine-tuning

    # Build the new model
    model = Sequential([
        base_model,
        Flatten(),
        Dense(
            units=hp.Int("units", min_value=64, max_value=512, step=64),
            activation="relu"
        ),
        Dropout(rate=hp.Float("dropout", min_value=0.2, max_value=0.5, step=0.1)),
        Dense(train_generator.num_classes, activation="softmax")
    ])

    # Compile the model with a tunable learning rate
    model.compile(
        optimizer=Adam(
            learning_rate=hp.Float("learning_rate", min_value=1e-5, max_value=1e-3, sampling="log")
        ),
        loss="categorical_crossentropy",
        metrics=["accuracy"]
    )

    return model


# Initialize Keras Tuner with RandomSearch
tuner = RandomSearch(
    build_model,
    objective="val_accuracy",
    max_trials=10,
    executions_per_trial=2,
    directory="hyperparameter_tuning",
    project_name="mobilenetv2_tuning"
)

# Callback to save the model for each epoch
checkpoint_callback = tf.keras.callbacks.ModelCheckpoint(
    filepath=os.path.join(fine_tuned_models_dir, "fine_tuned_model_epoch_{epoch:02d}.keras"),
    save_weights_only=False,  # Save the full model
    save_best_only=False,  # Save the model at every epoch
    verbose=1
)

# Run the search
tuner.search(
    train_generator,
    validation_data=val_generator,
    epochs=10,
    callbacks=[
        checkpoint_callback,
        tf.keras.callbacks.EarlyStopping(monitor="val_loss", patience=3)
    ]
)

# Get the best hyperparameters and model
best_hps = tuner.get_best_hyperparameters(num_trials=1)[0]
best_model = tuner.get_best_models(num_models=1)[0]

# Save the best model as a .keras file
best_model.save(os.path.join(fine_tuned_models_dir, "best_mobilenetv2_model.keras"))

# Evaluate the best model on the validation and test datasets
val_loss, val_accuracy = best_model.evaluate(val_generator)
test_loss, test_accuracy = best_model.evaluate(test_generator)

print(f"Validation Loss: {val_loss}, Validation Accuracy: {val_accuracy}")
print(f"Test Loss: {test_loss}, Test Accuracy: {test_accuracy}")

# Log hyperparameters and metrics to a CSV file
with open(summary_log_file, mode="w", newline="") as file:
    writer = csv.writer(file)
    writer.writerow(["Hyperparameter", "Value"])
    writer.writerow(["Units", best_hps.get("units")])
    writer.writerow(["Dropout", best_hps.get("dropout")])
    writer.writerow(["Learning Rate", best_hps.get("learning_rate")])
    writer.writerow(["Validation Loss", val_loss])
    writer.writerow(["Validation Accuracy", val_accuracy])
    writer.writerow(["Test Loss", test_loss])
    writer.writerow(["Test Accuracy", test_accuracy])

print("Summary saved to:", summary_log_file)
