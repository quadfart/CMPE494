from tensorflow.keras.preprocessing.image import ImageDataGenerator
from tensorflow.keras.applications import MobileNetV2
from tensorflow.keras.layers import Dense, GlobalAveragePooling2D
from tensorflow.keras.models import Model
from tensorflow.keras.callbacks import TensorBoard, ModelCheckpoint, Callback
import tensorflow as tf
from sklearn.metrics import classification_report, confusion_matrix
import numpy as np
import os
import time
import signal

# Parameters
IMG_SIZE = (224, 224)  # Image input size for the model
BATCH_SIZE = 128  # Batch size for training and evaluation
EPOCHS = 50  # Number of epochs for training
FINE_TUNE_EPOCHS = 20  # Additional epochs for fine-tuning
train_dir = "/home/quad/dataset_6000/train"
val_dir = "/home/quad/dataset_6000/val"
test_dir = "/home/quad/dataset_6000/test"
model_save_dir = "/home/quad/dataset_6000/models"
log_dir = "/home/quad/dataset_6000/models/logs/fit"
os.makedirs(log_dir, exist_ok=True)
os.makedirs(model_save_dir, exist_ok=True)

# Data generators
datagen = ImageDataGenerator(rescale=1.0 / 255.0)
train_generator = datagen.flow_from_directory(train_dir, target_size=IMG_SIZE, batch_size=BATCH_SIZE,
                                              class_mode="categorical")
validation_generator = datagen.flow_from_directory(val_dir, target_size=IMG_SIZE, batch_size=BATCH_SIZE,
                                                   class_mode="categorical")
test_generator = datagen.flow_from_directory(test_dir, target_size=IMG_SIZE, batch_size=BATCH_SIZE,
                                             class_mode="categorical", shuffle=False)

# Build the model
def build_model(base_model_fn, input_shape, num_classes, freeze_base=True):
    base_model = base_model_fn(weights="imagenet", include_top=False, input_shape=input_shape)
    base_model.trainable = not freeze_base
    x = base_model.output
    x = GlobalAveragePooling2D()(x)
    x = Dense(1024, activation="relu")(x)
    output = Dense(num_classes, activation="softmax")(x)
    model = Model(inputs=base_model.input, outputs=output)
    return model

model = build_model(MobileNetV2, IMG_SIZE + (3,), train_generator.num_classes, freeze_base=True)

# Compile the model
model.compile(optimizer=tf.keras.optimizers.Adam(learning_rate=0.001),
              loss="categorical_crossentropy", metrics=["accuracy"])

# Callbacks
tensorboard_callback = TensorBoard(log_dir=log_dir, histogram_freq=1)
best_model_checkpoint = ModelCheckpoint(
    filepath=os.path.join(model_save_dir, "best_model.keras"),
    save_best_only=True,
    monitor="val_accuracy",
    mode="max"
)

# Custom Callback for Manual Save and Stop
class ManualSaveStopCallback(Callback):
    def __init__(self, model_save_dir):
        super().__init__()
        self.model_save_dir = model_save_dir
        self.stop_training = False

    def on_epoch_end(self, epoch, logs=None):
        if self.stop_training:
            print("\n[INFO] Manual stop requested. Saving model and exiting...")
            model_save_path = os.path.join(self.model_save_dir, f"manual_stop_model_epoch_{epoch + 1}.keras")
            self.model.save(model_save_path)
            print(f"[INFO] Model saved to: {model_save_path}")
            self.model.stop_training = True

# Handle manual interruption
def handle_interrupt(signal, frame):
    print("\n[INFO] Ctrl+C detected. Stopping training at the end of the current epoch...")
    manual_callback.stop_training = True

signal.signal(signal.SIGINT, handle_interrupt)

manual_callback = ManualSaveStopCallback(model_save_dir)

# Train the model
print("\n[INFO] Starting training...")
history = model.fit(
    train_generator,
    epochs=EPOCHS,
    validation_data=validation_generator,
    callbacks=[tensorboard_callback, best_model_checkpoint, manual_callback]
)

# Save the final model after training
final_model_path = os.path.join(model_save_dir, "final_model_epoch_50.keras")
model.save(final_model_path)
print(f"[INFO] Final model saved to: {final_model_path}")

# Fine-tuning
print("\n[INFO] Starting fine-tuning...")
model.trainable = True
model.compile(optimizer=tf.keras.optimizers.Adam(learning_rate=1e-5),
              loss="categorical_crossentropy", metrics=["accuracy"])

fine_tune_history = model.fit(
    train_generator,
    epochs=FINE_TUNE_EPOCHS,
    validation_data=validation_generator,
    callbacks=[tensorboard_callback, best_model_checkpoint, manual_callback]
)

# Save the final fine-tuned model
final_fine_tuned_model_path = os.path.join(model_save_dir, "final_fine_tuned_model.keras")
model.save(final_fine_tuned_model_path)
print(f"[INFO] Final fine-tuned model saved to: {final_fine_tuned_model_path}")

# Evaluate on the test set
print("\n[INFO] Evaluating on the test set...")
test_loss, test_acc = model.evaluate(test_generator, verbose=1)

# Generate predictions for the test set
y_true = test_generator.classes
y_pred = np.argmax(model.predict(test_generator), axis=1)

# Classification report and confusion matrix
class_labels = list(test_generator.class_indices.keys())
clf_report = classification_report(y_true, y_pred, target_names=class_labels, output_dict=True)
conf_matrix = confusion_matrix(y_true, y_pred)

# Save test results to a file
results_file = os.path.join(model_save_dir, "test_results.txt")
with open(results_file, "w") as f:
    f.write(f"Test Accuracy: {test_acc:.4f}\n\n")
    f.write("Classification Report:\n")
    f.write(classification_report(y_true, y_pred, target_names=class_labels))
    f.write("\nConfusion Matrix:\n")
    f.write(np.array2string(conf_matrix, separator=", "))

print(f"\n[INFO] Test evaluation complete. Results saved to: {results_file}")
