import os
import random
import shutil

def copy_random_images(source_folder, target_folder, num_images, exclude_folder=None):

    if not os.path.exists(target_folder):
        os.makedirs(target_folder)

    all_directories = [d for d in os.listdir(source_folder) if os.path.isdir(os.path.join(source_folder, d))]

    if exclude_folder and exclude_folder in all_directories:
        all_directories.remove(exclude_folder)

    selected_images = []

    for directory in all_directories:
        directory_path = os.path.join(source_folder, directory)
        print(f"Checking directory: {directory_path}")
        images = [f for f in os.listdir(directory_path) if os.path.isfile(os.path.join(directory_path, f))]
        print(f"Images found in {directory}: {images}")

        if images:
            random_image = random.choice(images)
            selected_images.append(os.path.join(directory_path, random_image))


    images_to_copy = random.sample(selected_images, min(num_images, len(selected_images)))
    print(f"Images to copy: {images_to_copy}")

    for image in images_to_copy:
        shutil.copy(image, os.path.join(target_folder, os.path.basename(image)))
        print(f"Copied {os.path.basename(image)} to {target_folder}")

source_folder = "E:/plantnet_300K/plantnet_300K/images/val"
target_folder = "E:/Pelargonium Zonale/validate/species_not_present"
exclude_folder = "E:/plantnet_300K/plantnet_300K/images/val/1355978"
copy_random_images(source_folder, target_folder, num_images=60, exclude_folder=exclude_folder)