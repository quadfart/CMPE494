import tensorflow as tf
from tensorflow.keras.models import load_model

# Load the model
model = load_model('/home/quad/CMPE494/Tensorflow/tensor/fine_tuned_model_epoch_08.keras')

# Initialize the converter
converter = tf.lite.TFLiteConverter.from_keras_model(model)

# Enable TensorFlow ops support
converter.target_spec.supported_ops = [
    tf.lite.OpsSet.TFLITE_BUILTINS,  # Native TensorFlow Lite operations
    tf.lite.OpsSet.SELECT_TF_OPS     # TensorFlow ops
]

# Convert the model
tflite_model = converter.convert()

# Save the converted model
with open('model_op.tflite', 'wb') as f:
    f.write(tflite_model)

print("Model successfully converted and saved as model_op.tflite")
