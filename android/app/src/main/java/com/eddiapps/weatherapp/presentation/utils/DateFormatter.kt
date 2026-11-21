package com.eddiapps.weatherapp.presentation.utils

import android.util.Log
import java.time.OffsetDateTime
import java.time.ZoneId
import java.time.format.DateTimeFormatter
import java.util.Locale

object DateFormatter {

    private val formatter = DateTimeFormatter.ofPattern(
        "dd.MM.yyyy, HH:mm 'Uhr'",
        Locale.GERMANY
    )

    fun format(utc: String): String {
        Log.d("TIMEZONE", ZoneId.systemDefault().toString())
        return OffsetDateTime.parse(utc)
            .atZoneSameInstant(ZoneId.systemDefault())
            .format(formatter)
    }
}