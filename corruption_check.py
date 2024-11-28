import os
import tensorflow as tf


def check_images_with_tensorflow(directory):

    problematic_images = []

    for root, _, files in os.walk(directory):
        for file in files:
            file_path = os.path.join(root, file)
            try:
                # Try to load the image
                image_data = tf.io.read_file(file_path)
                _ = tf.image.decode_image(image_data)
            except Exception as e:
                problematic_images.append(file_path)
                print(f"Problematic image detected: {file_path}. Error: {e}")
                os.remove(file_path)  # Optionally, delete the problematic image

    print(f"Total problematic images removed: {len(problematic_images)}")


# Path to your dataset
dataset_directory = '/home/quad/autotest_dataset_1200'
check_images_with_tensorflow(dataset_directory)
