import os

def get_class_names(dataset_dir):
    """
    Retrieves class names from the dataset directory structure.

    Args:
        dataset_dir (str): Path to the dataset directory.

    Returns:
        list: A sorted list of class names.
    """
    # List all subdirectories in the dataset directory
    class_names = sorted([
        d for d in os.listdir(dataset_dir)
        if os.path.isdir(os.path.join(dataset_dir, d))
    ])
    return class_names

# Example usage
dataset_directory = "/home/quad/dataset_6000/test"  # Replace with your dataset directory path
class_names = get_class_names(dataset_directory)

# Output class names
print("Class names:", class_names)