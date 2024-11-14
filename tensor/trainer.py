import tensorflow as tf
from tensorflow.keras.preprocessing.image import ImageDataGenerator

# Set the path to your dataset
dataset_path = "E:/Pelargonium Zonale"

# Create ImageDataGenerators for training and validation
train_datagen = ImageDataGenerator(
    rescale=1./255,  # Normalize pixel values to [0, 1]
    rotation_range=20,
    width_shift_range=0.2,
    height_shift_range=0.2,
    shear_range=0.2,
    zoom_range=0.2,
    horizontal_flip=True,
    fill_mode='nearest'
)

val_datagen = ImageDataGenerator(rescale=1./255)

# Flow images from the directories
train_generator = train_datagen.flow_from_directory(
    directory=dataset_path + '/train',  # Path for training images
    target_size=(150, 150),  # Resize images to a standard size
    batch_size=32,
    class_mode='binary'  # Binary classification
)

validation_generator = val_datagen.flow_from_directory(
    directory=dataset_path + '/val',  # Path for validation images
    target_size=(150, 150),
    batch_size=32,
    class_mode='binary'
)