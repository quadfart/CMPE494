import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'welcome_page.dart'; // Replace with your actual welcome page import

class ProfilePage extends StatelessWidget {
  final Map<String, dynamic> user;

  // Accepting user data via constructor
  ProfilePage({required this.user});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Profile"),
      ),
      body: Padding(
        padding: const EdgeInsets.all(20.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const SizedBox(height: 40),
            // Display user name
            Text(
              "Welcome, ${user['name']}!",
              style: const TextStyle(
                fontSize: 24,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 20),
            // Display user email
            Text(
              "Email: ${user['email']}",
              style: const TextStyle(
                fontSize: 18,
              ),
            ),
            const SizedBox(height: 20),
            // Display user ID
            Text(
              "User ID: ${user['id']}",
              style: const TextStyle(
                fontSize: 18,
              ),
            ),
            const SizedBox(height: 40),

            // Button to Identify Page
            ElevatedButton(
              onPressed: () {
                // Navigate to IdentifyPage
                Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (context) => ProfilePage(user: user),
                  ),
                );
              },
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.blue,
                padding: const EdgeInsets.symmetric(vertical: 15),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(10),
                ),
              ),
              child: const Text(
                "Identify Plants",
                style: TextStyle(fontSize: 18, color: Colors.white),
              ),
            ),
            const Spacer(),

            // Logout Button
            ElevatedButton(
              onPressed: () async {
                // Clear SharedPreferences to remove login data
                SharedPreferences prefs = await SharedPreferences.getInstance();
                await prefs.clear(); // Clear all saved data

                // Navigate to the Welcome Page
                Navigator.pushReplacement(
                  context,
                  MaterialPageRoute(builder: (context) => const WelcomePage()), // Replace with your actual WelcomePage
                );
              },
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.red,
                padding: const EdgeInsets.symmetric(vertical: 15),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(10),
                ),
              ),
              child: const Text(
                "Logout",
                style: TextStyle(fontSize: 18, color: Colors.white),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
