import os
import numpy as np
import pandas as pd
import tensorflow as tf
from tensorflow.keras.models import load_model
from tensorflow.keras.preprocessing import image
from tensorflow.keras.preprocessing.image import img_to_array
from sklearn.metrics import confusion_matrix
import seaborn as sns
import matplotlib.pyplot as plt
from tqdm import tqdm
from datetime import datetime

# Path to your trained model
model_path = '/home/quad/CMPE494/Tensorflow/tensor/fine_tuned_model_epoch_08.keras'

# Path to the directory containing your test images
test_dir = '/home/quad/dataset_6000/test'

# TensorBoard log directory
log_dir = "/home/quad/retrain_dataset_6000/conf" + datetime.now().strftime("%Y%m%d-%H%M%S")
file_writer_cm = tf.summary.create_file_writer(log_dir)

# Load the model
model = load_model(model_path)

# Get class names from the directory names
class_names = sorted(os.listdir(test_dir))

# Function to preprocess the image and make a prediction
def predict_image(img_path):
    img = image.load_img(img_path, target_size=(224, 224))  # Adjust target size as per your model's input
    img_array = img_to_array(img)
    img_array = np.expand_dims(img_array, axis=0)  # Add batch dimension
    img_array = img_array / 255.0  # Normalize the image if needed
    predictions = model.predict(img_array)
    predicted_class_idx = np.argmax(predictions, axis=-1)
    return predicted_class_idx[0]  # Return class index

# Function to plot confusion matrix
def plot_confusion_matrix(cm, class_names):
    figure = plt.figure(figsize=(10, 8))
    sns.heatmap(cm, annot=True, fmt="d", cmap="Blues", xticklabels=class_names, yticklabels=class_names)
    plt.title("Confusion Matrix")
    plt.ylabel("True Label")
    plt.xlabel("Predicted Label")
    plt.close(figure)  # Close the plot to avoid displaying it in notebooks
    return figure

# Function to convert Matplotlib figure to TensorFlow image
def plot_to_image(figure):
    import io
    buf = io.BytesIO()
    plt.savefig(buf, format='png')
    plt.close(figure)
    buf.seek(0)
    image = tf.image.decode_png(buf.getvalue(), channels=4)
    return tf.expand_dims(image, 0)

# Function to run predictions on all images in the test directory
def test_model_and_log_cm(test_dir, class_names):
    true_labels = []
    predicted_labels = []

    # Iterate through all the images in the directory
    for root, dirs, files in tqdm(os.walk(test_dir), desc="Testing images", unit="image"):
        for file in files:
            if file.endswith(('.png', '.jpg', '.jpeg')):  # Adjust as needed
                img_path = os.path.join(root, file)
                true_class_name = os.path.basename(root)  # Assuming folder name is the true class name
                true_class_idx = class_names.index(true_class_name)
                predicted_class_idx = predict_image(img_path)

                true_labels.append(true_class_idx)
                predicted_labels.append(predicted_class_idx)

    # Calculate the confusion matrix
    cm = confusion_matrix(true_labels, predicted_labels)
    figure = plot_confusion_matrix(cm, class_names)

    # Log confusion matrix to TensorBoard
    with file_writer_cm.as_default():
        tf.summary.image("Confusion Matrix", plot_to_image(figure), step=0)

    print("Confusion matrix logged to TensorBoard.")

# Run the testing and logging function
test_model_and_log_cm(test_dir, class_names)
