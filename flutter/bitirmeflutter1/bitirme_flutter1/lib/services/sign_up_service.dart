import 'dart:convert';
import 'package:http/http.dart' as http;

class SignUpService {
  // Define the sign-up API endpoint
  static const String _baseUrl = 'http://192.168.31.100:5273/api/Auth/SignUp';

  // Function to handle the sign-up request
  Future<Map<String, dynamic>> signUp(String name, String email, String password) async {
    // Prepare the request body
    final Map<String, String> requestBody = {
      'name': name,
      'email': email,
      'password': password,
    };

    try {
      final response = await http.post(
        Uri.parse(_baseUrl),
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json',
        },
        body: json.encode(requestBody),
      );

      // Check the response status
      if (response.statusCode == 200) {
        // Parse the response if the sign-up is successful
        final responseData = json.decode(response.body);
        return {
          'success': true,
          'message': responseData['data'],
        };
      } else {
        // If the server responds with an error
        return {
          'success': false,
          'message': 'Failed to sign up. Please try again.',
        };
      }
    } catch (e) {
      // Handle any errors (e.g., network issues)
      return {
        'success': false,
        'message': 'An error occurred: $e',
      };
    }
  }
}
