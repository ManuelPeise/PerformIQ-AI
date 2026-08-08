package com.performiq.healthconnectapp

import android.os.Bundle
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity
import com.performiq.healthconnectbridge.HealthConnectBridge

class MainActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        val statusText = findViewById<TextView>(R.id.statusText)
        val isSupported = HealthConnectBridge.isSdkSupported()
        val isAvailable = HealthConnectBridge.isHealthConnectAvailable(this)
        statusText.text = getString(
            R.string.health_connect_status,
            isSupported.toString(),
            isAvailable.toString()
        )
    }
}
