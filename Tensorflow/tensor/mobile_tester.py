import tensorflow as tf
model = tf.saved_model.load("saved_model")
print(model.signatures)

