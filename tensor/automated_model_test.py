from tensorflow.keras.preprocessing.image import ImageDataGenerator
from tensorflow.keras.applications import (
    MobileNetV2, MobileNetV3Small, EfficientNetB0, InceptionV3, ResNet50, NASNetMobile
)
from tensorflow.keras.layers import Dense, GlobalAveragePooling2D
from tensorflow.keras.models import Model
from tensorflow.keras.callbacks import TensorBoard
from sklearn.metrics import classification_report, confusion_matrix
import tensorflow as tf
import pandas as pd
import numpy as np
import os
import time

# Parameters
IMG_SIZE = (224,224)  # Image input size for models
BATCH_SIZE = 64  # Batch size for training and evaluation
EPOCHS = 15  # Number of epochs for training
FINE_TUNE_EPOCHS = 0  # Additional epochs for fine-tuning
MODELS = {
    "MobileNetV2": MobileNetV2,
    "MobileNetV3Small": MobileNetV3Small,
    "EfficientNetB0": EfficientNetB0,
    "InceptionV3": InceptionV3,
    "ResNet50": ResNet50,
    "NASNetMobile": NASNetMobile,
}

# Dataset directories
train_dir = "/home/quad/autotest_dataset_1200/train"
val_dir = "/home/quad/autotest_dataset_1200/val"
test_dir = "/home/quad/autotest_dataset_1200/test"

# Model save path
model_save_dir = "/home/quad/autotest_dataset_1200/models"
log_dir = "/home/quad/autotest_dataset_1200/models/logs/fit"
os.makedirs(log_dir, exist_ok=True)
os.makedirs(model_save_dir, exist_ok=True)

# Data generators
datagen = ImageDataGenerator(rescale=1.0 / 255.0)

# Data generators for training, validation, and testing
train_generator = datagen.flow_from_directory(train_dir, target_size=IMG_SIZE, batch_size=BATCH_SIZE,
                                              class_mode="categorical")

validation_generator = datagen.flow_from_directory(val_dir, target_size=IMG_SIZE, batch_size=BATCH_SIZE,
                                                   class_mode="categorical")

test_generator = datagen.flow_from_directory(test_dir, target_size=IMG_SIZE, batch_size=BATCH_SIZE,
                                             class_mode="categorical", shuffle=False)

# Function to create a model
def build_model(base_model_fn, input_shape, num_classes, freeze_base=True):
    base_model = base_model_fn(weights="imagenet", include_top=False, input_shape=input_shape)
    base_model.trainable = not freeze_base  # Freeze or unfreeze the base model

    # Add custom classification layers
    x = base_model.output
    x = GlobalAveragePooling2D()(x)
    x = Dense(1024, activation="relu")(x)
    output = Dense(num_classes, activation="softmax")(x)
    model = Model(inputs=base_model.input, outputs=output)
    return model

# Dictionary to log metrics for all models
results_summary = []
results_detailed = []

# Train and evaluate each model
for model_name, model_fn in MODELS.items():
    print(f"\n--- Training {model_name} ---")

    # Build and compile the model (initial training with frozen base)
    model = build_model(model_fn, IMG_SIZE + (3,), train_generator.num_classes, freeze_base=True)
    model.compile(optimizer=tf.keras.optimizers.Adam(learning_rate=0.001),
                  loss="categorical_crossentropy", metrics=["accuracy"])
    tensorboard_callback=TensorBoard(log_dir=os.path.join(log_dir,f'{model_name}'),histogram_freq=1)
    # Train the classification head
    start_time = time.time()
    history = model.fit(
        train_generator,
        epochs=EPOCHS,
        validation_data=validation_generator,
        verbose=1,
        callbacks=[tensorboard_callback]
    )
    training_time = time.time() - start_time
    '''
    # Fine-tune the base model
    print(f"\n--- Fine-tuning {model_name} ---")
    model.trainable = True  # Unfreeze the entire model
    model.compile(optimizer=tf.keras.optimizers.Adam(learning_rate=1e-5),
                  loss="categorical_crossentropy", metrics=["accuracy"])

    fine_tune_history = model.fit(
        train_generator,
        epochs=FINE_TUNE_EPOCHS,
        validation_data=validation_generator,
        verbose=1
    )
    '''
    training_time += time.time() - start_time

    # Evaluate the model
    test_loss, test_acc = model.evaluate(test_generator, verbose=1)

    # Generate predictions for the test set
    y_true = test_generator.classes
    y_pred = np.argmax(model.predict(test_generator), axis=1)

    # Classification report and confusion matrix
    class_labels = list(test_generator.class_indices.keys())
    clf_report = classification_report(y_true, y_pred, target_names=class_labels, output_dict=True)
    conf_matrix = confusion_matrix(y_true, y_pred)

    # Save the model
    model_save_path = os.path.join(model_save_dir, f"{model_name}_model.keras")
    model.save(model_save_path)

    # Save results for the summary CSV
    results_summary.append({
        "Model": model_name,
        "Test Accuracy": test_acc,
        "Training Time (s)": training_time,
        "Precision (macro avg)": clf_report["macro avg"]["precision"],
        "Recall (macro avg)": clf_report["macro avg"]["recall"],
        "F1-Score (macro avg)": clf_report["macro avg"]["f1-score"],
        "Model Save Path": model_save_path
    })

    # Save detailed metrics for further analysis
    for class_name in class_labels:
        results_detailed.append({
            "Model": model_name,
            "Class": class_name,
            "Precision": clf_report[class_name]["precision"],
            "Recall": clf_report[class_name]["recall"],
            "F1-Score": clf_report[class_name]["f1-score"],
            "Confusion Matrix": conf_matrix[class_labels.index(class_name)].tolist()  # Row from the confusion matrix
        })

    # Clear session to free memory
    tf.keras.backend.clear_session()

# Save summary results to a CSV file
df_results_summary = pd.DataFrame(results_summary)
df_results_summary.to_csv(os.path.join(model_save_dir, "model_comparison_summary.csv"), index=False)

# Save detailed results to a CSV file
df_results_detailed = pd.DataFrame(results_detailed)
df_results_detailed.to_csv(os.path.join(model_save_dir, "model_comparison_detailed.csv"), index=False)

# Print results
print("Training and evaluation completed. Results saved.")
