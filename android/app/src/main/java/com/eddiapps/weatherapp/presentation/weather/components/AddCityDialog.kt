package com.eddiapps.weatherapp.presentation.weather.components

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.heightIn
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material3.AlertDialog
import androidx.compose.material3.LinearProgressIndicator
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.Text
import androidx.compose.material3.TextButton
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.eddiapps.weatherapp.presentation.weather.AddCityUiState

@Composable
fun AddCityDialog(
    state: AddCityUiState,
    onSearch: (String) -> Unit,
    onAddCity: (Int) -> Unit,
    onDismiss: () -> Unit
) {
    var query by remember { mutableStateOf("") }
    var lastSearchedQuery by remember { mutableStateOf<String?>(null) }

    val isBusy = state.isSearching || state.isAddingCity
    val showResults = lastSearchedQuery == query.trim()

    AlertDialog(
        onDismissRequest = onDismiss,
        title = { Text("Stadt hinzufügen") },
        text = {
            Column(
                verticalArrangement = Arrangement.spacedBy(12.dp)
            ) {
                OutlinedTextField(
                    value = query,
                    onValueChange = {
                        query = it
                        lastSearchedQuery = null
                    },
                    label = { Text("Stadt suchen") },
                    singleLine = true,
                    enabled = !isBusy,
                    modifier = Modifier.fillMaxWidth()
                )

                if (isBusy) {
                    LinearProgressIndicator(
                        modifier = Modifier.fillMaxWidth()
                    )
                }

                state.searchErrorMessage?.let {
                    Text(
                        text = it,
                        color = MaterialTheme.colorScheme.error
                    )
                }

                state.addCityErrorMessage?.let {
                    Text(
                        text = it,
                        color = MaterialTheme.colorScheme.error
                    )
                }

                if (showResults) {
                    if (state.hasSearchedCities && state.searchResults.isEmpty()) {
                        Text("Keine Städte gefunden.")
                    }

                    LazyColumn(
                        modifier = Modifier.heightIn(max = 250.dp)
                    ) {
                        items(
                            items = state.searchResults,
                            key = { it.locationId }
                        ) { city ->
                            TextButton(
                                onClick = { onAddCity(city.locationId) },
                                enabled = !isBusy,
                                modifier = Modifier.fillMaxWidth()
                            ) {
                                Column(
                                    modifier = Modifier.fillMaxWidth()
                                ) {
                                    Text(city.name)

                                    Text(
                                        text = listOfNotNull(
                                            city.region,
                                            city.country
                                        ).joinToString(", "),
                                        style = MaterialTheme.typography.bodySmall
                                    )
                                }
                            }
                        }
                    }
                }
            }
        },
        confirmButton = {
            TextButton(
                onClick = {
                    lastSearchedQuery = query.trim()
                    onSearch(query)
                },
                enabled = query.isNotBlank() && !isBusy
            ) {
                Text("Suchen")
            }
        },
        dismissButton = {
            TextButton(
                onClick = onDismiss,
                enabled = !isBusy
            ) {
                Text("Schließen")
            }
        }
    )
}
