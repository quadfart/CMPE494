import os
import random
import shutil
import time
from PIL import Image

# Path to the parent directory containing subdirectories with images
parent_dir = '/home/quad/dataset/train'
target_dir = '/home/quad/autotest_dataset_1200'
# Target number of images per folder (for undersampling)
target_num = 1200  # You can change this number as needed

# Split percentages for train, test, and val
train_pct = 0.8
test_pct = 0.1
val_pct = 0.1

# Create directories for train, test, and validation
train_dir = os.path.join(target_dir, 'train')
test_dir = os.path.join(target_dir, 'test')
val_dir = os.path.join(target_dir, 'val')

# Make sure the directories exist
for dir in [train_dir, test_dir, val_dir]:
    os.makedirs(dir, exist_ok=True)

# Function to verify if an image is valid
def is_valid_image(image_path):
    try:
        with Image.open(image_path) as img:
            img.verify()  # Check for corruption
        return True
    except (IOError, SyntaxError):
        return False

# Function to copy files reliably
def reliable_copy(src, dest):
    with open(src, 'rb') as f_src:
        with open(dest, 'wb') as f_dest:
            while chunk := f_src.read(1024 * 1024):  # Read in 1MB chunks
                f_dest.write(chunk)
    time.sleep(0.01)  # Add a small delay to throttle disk I/O

# Loop over each subdirectory (representing a class in your dataset)
for class_name in os.listdir(parent_dir):
    class_path = os.path.join(parent_dir, class_name)

    if os.path.isdir(class_path):
        # List all valid images in the class folder
        images = [
            img for img in os.listdir(class_path)
            if img.lower().endswith(('jpg', 'jpeg')) and is_valid_image(os.path.join(class_path, img))
        ]

        # Ensure each class has exactly 'target_num' images
        if len(images) > target_num:
            images = random.sample(images, target_num)
        elif len(images) < target_num:
            print(f"Warning: {class_name} has fewer than {target_num} valid images, skipping undersampling.")

        # Split the images into train, test, and val sets
        num_train = int(len(images) * train_pct)
        num_test = int(len(images) * test_pct)
        num_val = len(images) - num_train - num_test

        train_images = images[:num_train]
        test_images = images[num_train:num_train + num_test]
        val_images = images[num_train + num_test:]

        # Create subdirectories for each class in train, test, and val directories
        class_train_dir = os.path.join(train_dir, class_name)
        class_test_dir = os.path.join(test_dir, class_name)
        class_val_dir = os.path.join(val_dir, class_name)

        os.makedirs(class_train_dir, exist_ok=True)
        os.makedirs(class_test_dir, exist_ok=True)
        os.makedirs(class_val_dir, exist_ok=True)

        # Copy images to corresponding directories
        for img in train_images:
            reliable_copy(os.path.join(class_path, img), os.path.join(class_train_dir, img))
        for img in test_images:
            reliable_copy(os.path.join(class_path, img), os.path.join(class_test_dir, img))
        for img in val_images:
            reliable_copy(os.path.join(class_path, img), os.path.join(class_val_dir, img))

print("Dataset splitting and copying completed!")
