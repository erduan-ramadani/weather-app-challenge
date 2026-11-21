package com.eddiapps.weatherapp.presentation.weather.components

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.RowScope
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.Card
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.eddiapps.weatherapp.domain.model.Weather
import com.eddiapps.weatherapp.presentation.utils.DateFormatter

@Composable
fun WeatherCard(
    weather: Weather,
    modifier: Modifier = Modifier
) {
    Card(modifier = modifier.fillMaxWidth()) {
        Column(
            modifier = Modifier.padding(20.dp),
            verticalArrangement = Arrangement.spacedBy(16.dp)
        ) {
            Text(
                text = weather.city,
                style = MaterialTheme.typography.titleLarge
            )

            Row(horizontalArrangement = Arrangement.spacedBy(16.dp)) {
                WeatherMetric(
                    label = "Temperatur",
                    value = "${weather.temperature} °C"
                )
                WeatherMetric(
                    label = "Regenwahrscheinlichkeit",
                    value = weather.rainProbability?.let { "$it %" } ?: "–"
                )
            }

            Row(horizontalArrangement = Arrangement.spacedBy(16.dp)) {
                WeatherMetric(
                    label = "Luftfeuchtigkeit",
                    value = "${weather.humidity} %"
                )
                WeatherMetric(
                    label = "Windgeschwindigkeit",
                    value = "${weather.windSpeed} km/h"
                )
            }

            Text(
                text = "Zuletzt aktualisiert: ${DateFormatter.format(weather.lastUpdatedUtc)}",
                style = MaterialTheme.typography.bodySmall
            )
        }
    }
}

@Composable
private fun RowScope.WeatherMetric(
    label: String,
    value: String
) {
    Column(
        modifier = Modifier.weight(1f),
        verticalArrangement = Arrangement.spacedBy(4.dp)
    ) {
        Text(
            text = label,
            style = MaterialTheme.typography.labelMedium
        )
        Text(
            text = value,
            style = MaterialTheme.typography.titleMedium
        )
    }
}
