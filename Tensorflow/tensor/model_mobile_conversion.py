from tensorflow.keras.models import load_model
import tensorflow as tf

model = load_model('final_fine_tuned_model.keras')

converter = tf.lite.TFLiteConverter.from_keras_model(model)
tflite_model = converter.convert()

# Save the TFLite model to a file
with open('model.tflite', 'wb') as f:
    f.write(tflite_model)
