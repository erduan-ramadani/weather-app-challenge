package com.eddiapps.weatherapp.domain.exception

class BackendUnavailableException(
    cause: Throwable
) : Exception("Backend unavailable", cause)
