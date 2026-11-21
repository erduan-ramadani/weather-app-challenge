package com.eddiapps.weatherapp.presentation.weather.components

import androidx.compose.material3.AlertDialog
import androidx.compose.material3.Text
import androidx.compose.material3.TextButton
import androidx.compose.runtime.Composable

@Composable
fun DeleteCityDialog(
    cityName: String,
    onConfirm: () -> Unit,
    onDismiss: () -> Unit
) {
    AlertDialog(
        onDismissRequest = onDismiss,
        title = { Text("Stadt löschen?") },
        text = {
            Text("Möchtest du $cityName wirklich löschen?")
        },
        confirmButton = {
            TextButton(onClick = onConfirm) {
                Text("Löschen")
            }
        },
        dismissButton = {
            TextButton(onClick = onDismiss) {
                Text("Abbrechen")
            }
        }
    )
}
