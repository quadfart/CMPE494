import os
import random
import shutil
import cv2
from tqdm import tqdm
from tensorflow.keras.preprocessing.image import ImageDataGenerator

def copy_random_images(src_dir, temp_dir, target_count=6000):
    """Copy up to target_count random images from each class to a temp directory."""
    os.makedirs(temp_dir, exist_ok=True)
    for class_name in tqdm(os.listdir(src_dir), desc="Copying images"):
        class_path = os.path.join(src_dir, class_name)
        if not os.path.isdir(class_path):
            continue

        temp_class_dir = os.path.join(temp_dir, class_name)
        os.makedirs(temp_class_dir, exist_ok=True)

        images = [os.path.join(class_path, f) for f in os.listdir(class_path) if f.endswith(('png', 'jpg', 'jpeg'))]
        selected_images = random.sample(images, min(len(images), target_count))
        for img_path in selected_images:
            shutil.copy(img_path, os.path.join(temp_class_dir, os.path.basename(img_path)))

def augment_images(temp_dir, target_count=6000, max_augmentations_per_image=3):
    """Augment images in classes with fewer than target_count images."""
    datagen = ImageDataGenerator(
        rotation_range=30,
        width_shift_range=0.2,
        height_shift_range=0.2,
        shear_range=0.2,
        zoom_range=0.2,
        horizontal_flip=True,
        fill_mode='nearest'
    )

    for class_name in tqdm(os.listdir(temp_dir), desc="Augmenting images"):
        class_path = os.path.join(temp_dir, class_name)
        if not os.path.isdir(class_path):
            continue

        images = [os.path.join(class_path, f) for f in os.listdir(class_path) if f.endswith(('png', 'jpg', 'jpeg'))]
        current_count = len(images)

        if current_count < target_count:
            augment_needed = target_count - current_count
            while augment_needed > 0:
                for img_path in images:
                    if augment_needed <= 0:
                        break
                    img = cv2.imread(img_path)
                    img = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
                    img = cv2.resize(img, (224, 224))  # Adjust size if needed

                    aug_imgs = augment_image(img, datagen, max_augmentations_per_image)
                    for aug_img in aug_imgs:
                        if augment_needed <= 0:
                            break
                        aug_img_path = os.path.join(class_path, f"{os.path.splitext(os.path.basename(img_path))[0]}_aug_{augment_needed}.jpg")
                        cv2.imwrite(aug_img_path, cv2.cvtColor(aug_img, cv2.COLOR_RGB2BGR))
                        augment_needed -= 1

def augment_image(img, datagen, max_augmentations_per_image):
    """Generate a limited number of augmented images from a single image."""
    img = img.reshape((1,) + img.shape)
    augmented_images = []
    for batch in datagen.flow(img, batch_size=1):
        augmented_images.append(batch[0].astype('uint8'))
        if len(augmented_images) >= max_augmentations_per_image:
            break
    return augmented_images

def split_dataset(temp_dir, target_dir, train_ratio=0.8, test_ratio=0.1, val_ratio=0.1):
    """Split the dataset into train, test, and val directories."""
    subsets = ['train', 'test', 'val']
    for subset in subsets:
        subset_path = os.path.join(target_dir, subset)
        os.makedirs(subset_path, exist_ok=True)

    for class_name in tqdm(os.listdir(temp_dir), desc="Splitting dataset"):
        class_path = os.path.join(temp_dir, class_name)
        if not os.path.isdir(class_path):
            continue

        images = [os.path.join(class_path, f) for f in os.listdir(class_path) if f.endswith(('png', 'jpg', 'jpeg'))]
        random.shuffle(images)

        train_end = int(len(images) * train_ratio)
        test_end = train_end + int(len(images) * test_ratio)

        split_images = {
            'train': images[:train_end],
            'test': images[train_end:test_end],
            'val': images[test_end:]
        }

        for subset, subset_images in split_images.items():
            subset_class_dir = os.path.join(target_dir, subset, class_name)
            os.makedirs(subset_class_dir, exist_ok=True)
            for img_path in subset_images:
                shutil.copy(img_path, os.path.join(subset_class_dir, os.path.basename(img_path)))

def clean_temp_directory(temp_dir):
    """Delete the temporary directory."""
    shutil.rmtree(temp_dir)

def count_images_per_class(target_dir):
    """Count and print the number of images per class in each subset."""
    for subset in ['train', 'test', 'val']:
        subset_path = os.path.join(target_dir, subset)
        print(f"\nImage counts for {subset}:")
        for class_name in os.listdir(subset_path):
            class_path = os.path.join(subset_path, class_name)
            if os.path.isdir(class_path):
                image_count = len([f for f in os.listdir(class_path) if f.endswith(('png', 'jpg', 'jpeg'))])
                print(f"  {class_name}: {image_count}")

if __name__ == "__main__":
    source_dir = "/home/quad/dataset/train"  # Source directory
    temp_dir = "/home/quad/6000temp"  # Temporary directory for processing
    target_dir = "/home/quad/dataset_6000"  # Final target directory
    target_images_per_class = 6000  # Target images per class

    print("Step 1: Copying random images to temp...")
    copy_random_images(source_dir, temp_dir, target_images_per_class)

    print("\nStep 2: Augmenting images to reach target...")
    augment_images(temp_dir, target_images_per_class)

    print("\nStep 3: Splitting dataset into train, test, and val...")
    split_dataset(temp_dir, target_dir)

    print("\nStep 4: Cleaning up temporary directory...")
    clean_temp_directory(temp_dir)

    print("\nStep 5: Counting images in final dataset...")
    count_images_per_class(target_dir)
