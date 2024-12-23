import os
import numpy as np
import pandas as pd
from tensorflow.keras.models import load_model
from tensorflow.keras.preprocessing import image
from tensorflow.keras.preprocessing.image import img_to_array
from tqdm import tqdm  # Optional, for progress bar

# Path to your trained model
model_path = '/home/quad/CMPE494/Tensorflow/tensor/fine_tuned_model_epoch_08.keras'

# Path to the directory containing your test images
test_dir = '/home/quad/dataset_6000/test'

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
    return predicted_class_idx[0], predictions[0][predicted_class_idx[0]]  # Return class index and confidence


# Function to run predictions on all images in the test directory
def test_model(test_dir, class_names):
    results = []
    # Iterate through all the images in the directory
    for root, dirs, files in tqdm(os.walk(test_dir), desc="Testing images", unit="image"):
        for file in files:
            if file.endswith(('.png', '.jpg', '.jpeg')):  # Adjust as needed
                img_path = os.path.join(root, file)
                class_idx, confidence = predict_image(img_path)
                predicted_class_name = class_names[class_idx]
                results.append([file, predicted_class_name, confidence])

    # Save the results to a CSV file
    results_df = pd.DataFrame(results, columns=['Image Filename', 'Predicted Class', 'Confidence'])
    results_df.to_csv('prediction_results.csv', index=False)
    print("Testing completed. Results saved to prediction_results.csv")


# Run the testing function
test_model(test_dir, class_names)
