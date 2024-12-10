import tensorflow as tf
from tensorflow.keras.preprocessing.image import ImageDataGenerator

# Set image size
image_size = (224, 224)  # Change this depending on your model's input requirements

# Set up ImageDataGenerators for training, validation, and test data
train_datagen = ImageDataGenerator(rescale=1./255, horizontal_flip=True, zoom_range=0.2, shear_range=0.2)
test_datagen = ImageDataGenerator(rescale=1./255)

train_generator = train_datagen.flow_from_directory(
    '/home/quad/dataset/train',  # Your training data directory
    target_size=image_size,
    batch_size=32,
    class_mode='categorical'
)

validation_generator = test_datagen.flow_from_directory(
    '/home/quad/dataset/val',  # Your validation data directory
    target_size=image_size,
    batch_size=32,
    class_mode='categorical'
)

test_generator = test_datagen.flow_from_directory(
    '/home/quad/dataset/test',  # Your test data directory
    target_size=image_size,
    batch_size=32,
    class_mode='categorical'
)