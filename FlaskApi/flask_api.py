from io import BytesIO
from flask import Flask, request, jsonify
from tensorflow.keras.models import load_model
from tensorflow.keras.preprocessing.image import img_to_array, load_img
import numpy as np
import os

app = Flask(__name__)

# Load the model
model = load_model('app/fine_tuned_model_epoch_08.keras')
model_disease = load_model('app/disease_model_epoch_6.keras')
# Define the class labels (you can load these dynamically from your dataset directory)
class_labels = ['Agave attenuata', 'Anthurium andraeanum', 'Chlorophytum comosum', 'Dianthus carthusianorum',
                'Dracaena reflexa', 'Epiphyllum oxypetalum', 'Pilea Peperomioides', 'Sedum morganianum',
                'Sedum rubrotinctum', 'Sempervivum arachnoideum', 'Sempervivum tectorum', 'Zamioculcas zamiifolia',
                'aloe vera', 'aspidistra elaitor', 'begonia x semperflorens-cultorum', 'bougainvillea glabra',
                'bougainvillea spectabilis', 'buxus sempervirens', 'camelia spp', 'cattleya trianae', 'crassula ovata',
                'crassula perforata', 'dieffenbachia seguine', 'dracaena trifasciata', 'epipremnum aurerum',
                'ficus elastica', 'ficus lyrata', 'geranium sanguineum', 'hedera helix', 'hibiscus rosa-sinensis',
                'hydrangea macrophylla', 'jasminum officinale', 'lavandula angustifolia', 'lavandula stoechas',
                'magnolia grandiflora', 'monstera deliciosa', 'nephrolepis exaltata', 'nerium oleander',
                'opuntia humifusa', 'orchis italica', 'orchis mascula', 'oscularia deltoides', 'pelargonium x hybridum',
                'pelargonium zonale', 'philodendron hederaceum', 'rosa spp', 'salvia rosmarinus', 'schefflera arboricola',
                'schefflera digitata', 'spathiphyllum spp', 'vanilla planifolia', 'viola tricolor', 'viola x wittockiana',
                'wisteria sinensis']
disease_class_labels = ['healthy', 'powdery', 'rust']

# Preprocess the image
def preprocess_image(image, target_size):
    image = image.resize(target_size)  # Resize to match model input size
    image = img_to_array(image)  # Convert to numpy array
    image = np.expand_dims(image, axis=0)  # Add batch dimension
    image = image / 255.0  # Normalize pixel values to [0, 1]
    return image

@app.route('/predict', methods=['POST'])
def predict():
    try:
        if 'file' not in request.files:
            return jsonify({'error': 'No file provided'}), 400

        file = request.files['file']
        if not file or not file.filename:
            return jsonify({'error': 'Invalid file'}), 400

        # Convert FileStorage to BytesIO
        file_stream = BytesIO(file.read())

        # Load and preprocess the image
        image = load_img(file_stream, target_size=(224, 224))  # Update target size if needed
        image = preprocess_image(image, target_size=(224, 224))

        # Make predictions
        predictions = model.predict(image).flatten()
        results = {class_labels[i]: float(predictions[i]) for i in range(len(class_labels))}

        # Format the results to prevent scientific notation and ensure precision
        formatted_results = {label: f"{value:.6f}" for label, value in results.items()}

        return jsonify({'predictions': formatted_results}), 200

    except Exception as e:
        app.logger.error(f"Error during prediction: {str(e)}", exc_info=True)
        return jsonify({'error': str(e)}), 500

@app.route('/predictDisease', methods=['POST'])
def predictDisease():
    try:
        if 'file' not in request.files:
            return jsonify({'error': 'No file provided'}), 400

        file = request.files['file']
        if not file or not file.filename:
            return jsonify({'error': 'Invalid file'}), 400

        # Convert FileStorage to BytesIO
        file_stream = BytesIO(file.read())

        # Load and preprocess the image
        image = load_img(file_stream, target_size=(224, 224))  # Update target size if needed
        image = preprocess_image(image, target_size=(224, 224))

        # Make predictions
        predictions = model_disease.predict(image).flatten()
        results = {disease_class_labels[i]: float(predictions[i]) for i in range(len(disease_class_labels))}

        # Format the results to prevent scientific notation and ensure precision
        formatted_results = {label: f"{value:.6f}" for label, value in results.items()}

        return jsonify({'predictions': formatted_results}), 200

    except Exception as e:
        app.logger.error(f"Error during prediction: {str(e)}", exc_info=True)
        return jsonify({'error': str(e)}), 500

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)
