import os
import pandas as pd


def count_photos_in_directories(base_path, output_file=os.path.join("/home/quad/dataset/train","photo_count_log.csv")):
    """
    Count the number of photos in each subdirectory of the base_path and log the results.

    Parameters:
        base_path (str): Path to the base directory containing subdirectories.
        output_file (str): Path to save the CSV log file. Default is 'photo_count_log.csv'.
    """
    data = []

    for root, dirs, files in os.walk(base_path):
        # Only consider immediate subdirectories, skip nested ones
        if root == base_path:
            for dir_name in dirs:
                dir_path = os.path.join(base_path, dir_name)
                # Count the image files in the current directory
                image_count = sum(
                    1 for file in os.listdir(dir_path)
                    if file.lower().endswith(('.png', '.jpg', '.jpeg', '.bmp', '.gif', '.tiff'))
                )
                data.append({"Directory": dir_name, "Number of Photos": image_count})

    # Convert to DataFrame and save as CSV
    df = pd.DataFrame(data)
    df.to_csv(output_file, index=False)
    print(f"Photo count logged in '{output_file}'.")


# Specify the base directory path
base_directory = "/home/quad/dataset/train"
count_photos_in_directories(base_directory)
