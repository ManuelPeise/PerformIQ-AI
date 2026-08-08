package com.performiq.healthconnectbridge

import android.content.Context
import android.content.Intent
import android.os.Build
import androidx.health.connect.client.HealthConnectClient
import androidx.health.connect.client.PermissionController
import androidx.health.connect.client.permission.HealthPermission
import androidx.health.connect.client.records.ActiveCaloriesBurnedRecord
import androidx.health.connect.client.records.BasalBodyTemperatureRecord
import androidx.health.connect.client.records.BasalMetabolicRateRecord
import androidx.health.connect.client.records.BloodGlucoseRecord
import androidx.health.connect.client.records.BloodPressureRecord
import androidx.health.connect.client.records.BodyFatRecord
import androidx.health.connect.client.records.BodyTemperatureRecord
import androidx.health.connect.client.records.BoneMassRecord
import androidx.health.connect.client.records.CervicalMucusRecord
import androidx.health.connect.client.records.CyclingPedalingCadenceRecord
import androidx.health.connect.client.records.DistanceRecord
import androidx.health.connect.client.records.ElevationGainedRecord
import androidx.health.connect.client.records.ExerciseSessionRecord
import androidx.health.connect.client.records.FloorsClimbedRecord
import androidx.health.connect.client.records.HeartRateRecord
import androidx.health.connect.client.records.HeartRateVariabilityRmssdRecord
import androidx.health.connect.client.records.HeightRecord
import androidx.health.connect.client.records.HydrationRecord
import androidx.health.connect.client.records.IntermenstrualBleedingRecord
import androidx.health.connect.client.records.LeanBodyMassRecord
import androidx.health.connect.client.records.MenstruationFlowRecord
import androidx.health.connect.client.records.MenstruationPeriodRecord
import androidx.health.connect.client.records.NutritionRecord
import androidx.health.connect.client.records.OvulationTestRecord
import androidx.health.connect.client.records.OxygenSaturationRecord
import androidx.health.connect.client.records.PowerRecord
import androidx.health.connect.client.records.Record
import androidx.health.connect.client.records.RespiratoryRateRecord
import androidx.health.connect.client.records.RestingHeartRateRecord
import androidx.health.connect.client.records.SexualActivityRecord
import androidx.health.connect.client.records.SleepSessionRecord
import androidx.health.connect.client.records.SpeedRecord
import androidx.health.connect.client.records.StepsCadenceRecord
import androidx.health.connect.client.records.StepsRecord
import androidx.health.connect.client.records.TotalCaloriesBurnedRecord
import androidx.health.connect.client.records.Vo2MaxRecord
import androidx.health.connect.client.records.WeightRecord
import androidx.health.connect.client.records.WheelchairPushesRecord
import kotlinx.coroutines.runBlocking
import org.json.JSONArray
import org.json.JSONObject
import java.time.Instant

object HealthConnectBridge {
    const val PermissionRequestCode: Int = 45271

    private val ReadRecordTypes: Set<kotlin.reflect.KClass<out Record>> = buildReadRecordTypes()

    @JvmStatic
    fun isSdkSupported(): Boolean {
        return Build.VERSION.SDK_INT >= Build.VERSION_CODES.O_MR1
    }

    @JvmStatic
    fun isHealthConnectAvailable(context: Context): Boolean {
        return runCatching {
            HealthConnectClient.getSdkStatus(context) == HealthConnectClient.SDK_AVAILABLE
        }.getOrDefault(false)
    }

    @JvmStatic
    fun getRequiredPermissionsJson(): String {
        val array = JSONArray()
        getReadPermissions().forEach { permission -> array.put(permission) }
        return array.toString()
    }

    @JvmStatic
    fun createPermissionRequestIntent(context: Context): Intent? {
        if (!isHealthConnectAvailable(context)) {
            return null
        }

        return runCatching {
            val contract = PermissionController.createRequestPermissionResultContract()
            contract.createIntent(context, getReadPermissions())
        }.getOrNull()
    }

    @JvmStatic
    fun parsePermissionResultJson(resultCode: Int, intent: Intent?): String {
        return runCatching {
            val contract = PermissionController.createRequestPermissionResultContract()
            val grantedPermissions = contract.parseResult(resultCode, intent)
            JSONObject().put("grantedPermissions", JSONArray(grantedPermissions.toList())).toString()
        }.getOrElse {
            JSONObject()
                .put("grantedPermissions", JSONArray())
                .put("error", it.message ?: "Failed to parse permission result")
                .toString()
        }
    }

    @JvmStatic
    fun hasAllPermissions(context: Context): Boolean = runCatching {
        runBlocking {
            val grantedPermissions = getGrantedPermissions(context)
            grantedPermissions.containsAll(getReadPermissions())
        }
    }.getOrDefault(false)

    @JvmStatic
    fun getGrantedPermissionsJson(context: Context): String = runCatching {
        runBlocking {
            val grantedPermissions = getGrantedPermissions(context)
            JSONArray(grantedPermissions.toList()).toString()
        }
    }.getOrElse { "[]" }

    @JvmStatic
    fun readAllRecordsAsJson(context: Context, startEpochMsUtc: Long, endEpochMsUtc: Long): String {
        val startTime = Instant.ofEpochMilli(startEpochMsUtc)
        val endTime = Instant.ofEpochMilli(endEpochMsUtc)
        if (startTime.isAfter(endTime)) {
            return buildErrorPayload("Invalid time range: startTime is after endTime", startTime, endTime)
        }

        return runCatching {
            runBlocking {
                val readResult = HealthConnectRecordReader.readAllRecords(context, ReadRecordTypes, startTime, endTime)
                HealthConnectRecordNormalizer.normalize(readResult, startTime, endTime).toString()
            }
        }.getOrElse { throwable ->
            buildErrorPayload(throwable.message ?: "Failed to read records", startTime, endTime)
        }
    }

    private fun buildErrorPayload(message: String, startTime: Instant, endTime: Instant): String {
        return HealthConnectRecordNormalizer
            .normalize(emptyList(), startTime, endTime)
            .put("error", message)
            .toString()
    }

    private suspend fun getGrantedPermissions(context: Context): Set<String> {
        val client = HealthConnectClient.getOrCreate(context)
        return client.permissionController.getGrantedPermissions()
    }

    private fun getReadPermissions(): Set<String> {
        return ReadRecordTypes.mapTo(mutableSetOf()) { recordType ->
            HealthPermission.getReadPermission(recordType)
        }
    }

    private fun buildReadRecordTypes(): Set<kotlin.reflect.KClass<out Record>> {
        val recordTypes = mutableSetOf<kotlin.reflect.KClass<out Record>>(
            ActiveCaloriesBurnedRecord::class,
            BasalBodyTemperatureRecord::class,
            BasalMetabolicRateRecord::class,
            BloodGlucoseRecord::class,
            BloodPressureRecord::class,
            BodyFatRecord::class,
            BodyTemperatureRecord::class,
            BoneMassRecord::class,
            CervicalMucusRecord::class,
            CyclingPedalingCadenceRecord::class,
            DistanceRecord::class,
            ElevationGainedRecord::class,
            ExerciseSessionRecord::class,
            FloorsClimbedRecord::class,
            HeartRateRecord::class,
            HeartRateVariabilityRmssdRecord::class,
            HeightRecord::class,
            HydrationRecord::class,
            IntermenstrualBleedingRecord::class,
            LeanBodyMassRecord::class,
            MenstruationFlowRecord::class,
            MenstruationPeriodRecord::class,
            NutritionRecord::class,
            OvulationTestRecord::class,
            OxygenSaturationRecord::class,
            PowerRecord::class,
            RespiratoryRateRecord::class,
            RestingHeartRateRecord::class,
            SexualActivityRecord::class,
            SleepSessionRecord::class,
            SpeedRecord::class,
            StepsCadenceRecord::class,
            StepsRecord::class,
            TotalCaloriesBurnedRecord::class,
            Vo2MaxRecord::class,
            WeightRecord::class,
            WheelchairPushesRecord::class
        )

        val skinTemperatureRecordTypeName = "androidx.health.connect.client.records.SkinTemperatureRecord"
        try {
            val classType = Class.forName(skinTemperatureRecordTypeName).kotlin
            @Suppress("UNCHECKED_CAST")
            recordTypes.add(classType as kotlin.reflect.KClass<out Record>)
        } catch (_: ClassNotFoundException) {
            // Optional Health Connect record type that is not available on older runtimes.
        }

        return recordTypes
    }
}
