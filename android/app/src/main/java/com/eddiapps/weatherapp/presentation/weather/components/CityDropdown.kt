package com.eddiapps.weatherapp.presentation.weather.components

import androidx.compose.foundation.layout.Box
import androidx.compose.material3.DropdownMenu
import androidx.compose.material3.DropdownMenuItem
import androidx.compose.material3.OutlinedButton
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import com.eddiapps.weatherapp.domain.model.City

@Composable
fun CityDropdown(
    cities: List<City>,
    onCitySelected: (Int) -> Unit,
    enabled: Boolean = true
) {
    var expanded by remember { mutableStateOf(false) }
    val selectedCity = cities.firstOrNull { it.isSelected }

    Box {
        OutlinedButton(
            onClick = { expanded = true },
            enabled = enabled && cities.isNotEmpty()
        ) {
            Text("${selectedCity?.name ?: "Stadt auswählen"} ▾")
        }

        DropdownMenu(
            expanded = expanded,
            onDismissRequest = { expanded = false }
        ) {
            cities.forEach { city ->
                DropdownMenuItem(
                    text = { Text("${city.name}, ${city.country}") },
                    enabled = enabled && !city.isSelected,
                    onClick = {
                        expanded = false
                        onCitySelected(city.id)
                    }
                )
            }
        }
    }
}
