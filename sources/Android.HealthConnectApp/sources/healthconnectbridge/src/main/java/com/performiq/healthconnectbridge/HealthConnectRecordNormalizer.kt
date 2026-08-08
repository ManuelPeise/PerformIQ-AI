package com.performiq.healthconnectbridge

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
import org.json.JSONArray
import org.json.JSONObject
import java.time.Instant

internal object HealthConnectRecordNormalizer {
    fun normalize(records: List<Record>, startTime: Instant, endTime: Instant): JSONObject {
        val payload = JSONObject()
        payload.put("schemaVersion", "1.0")
        payload.put("generatedAtUtc", Instant.now().toString())
        payload.put("timeRange", buildTimeRange(startTime, endTime))
        payload.put("recordCount", records.size)
        payload.put("records", normalizeRecords(records))
        return payload
    }

    private fun buildTimeRange(startTime: Instant, endTime: Instant): JSONObject {
        val range = JSONObject()
        range.put("startTimeUtc", startTime.toString())
        range.put("endTimeUtc", endTime.toString())
        return range
    }

    private fun normalizeRecords(records: List<Record>): JSONArray {
        val result = JSONArray()
        records.forEach { record -> result.put(normalizeRecord(record)) }
        return result
    }

    private fun normalizeRecord(record: Record): JSONObject {
        val normalized = baseRecord(record)
        normalized.put("values", valuesByType(record))
        return normalized
    }

    private fun baseRecord(record: Record): JSONObject {
        val json = JSONObject()
        json.put("recordType", record::class.java.simpleName)
        addTemporalFields(json, record)
        json.put("sourceApp", record.metadata.dataOrigin.packageName)
        json.put("lastModifiedTimeUtc", record.metadata.lastModifiedTime.toString())
        return json
    }

    private fun addTemporalFields(json: JSONObject, record: Record) {
        // Health Connect exposes different temporal properties per record type.
        putIfPresent(json, "startTimeUtc", record, "getStartTime")
        putIfPresent(json, "endTimeUtc", record, "getEndTime")
        putIfPresent(json, "startZoneOffset", record, "getStartZoneOffset")
        putIfPresent(json, "endZoneOffset", record, "getEndZoneOffset")
        putIfPresent(json, "timeUtc", record, "getTime")
        putIfPresent(json, "zoneOffset", record, "getZoneOffset")
    }

    private fun putIfPresent(json: JSONObject, jsonKey: String, target: Any, getterName: String) {
        val method = target.javaClass.methods.firstOrNull { method ->
            method.name == getterName && method.parameterCount == 0
        } ?: return

        val value = method.invoke(target)
        json.put(jsonKey, value?.toString())
    }

    private fun valuesByType(record: Record): JSONObject {
        return when (record) {
            is ActiveCaloriesBurnedRecord -> json("energyKilocalories", record.energy.inKilocalories)
            is BasalBodyTemperatureRecord -> json("temperatureCelsius", record.temperature.inCelsius)
            is BasalMetabolicRateRecord -> json("powerWatts", record.basalMetabolicRate.inWatts)
            is BloodGlucoseRecord -> bloodGlucoseValues(record)
            is BloodPressureRecord -> bloodPressureValues(record)
            is BodyFatRecord -> json("percentage", record.percentage.value)
            is BodyTemperatureRecord -> json("temperatureCelsius", record.temperature.inCelsius)
            is BoneMassRecord -> json("massGrams", record.mass.inGrams)
            is CervicalMucusRecord -> json("appearance", record.appearance.toString()).put("sensation", record.sensation.toString())
            is CyclingPedalingCadenceRecord -> json("sampleCount", record.samples.size)
            is DistanceRecord -> json("distanceMeters", record.distance.inMeters)
            is ElevationGainedRecord -> json("elevationMeters", record.elevation.inMeters)
            is ExerciseSessionRecord -> exerciseValues(record)
            is FloorsClimbedRecord -> json("floors", record.floors)
            is HeartRateRecord -> heartRateValues(record)
            is HeartRateVariabilityRmssdRecord -> json("rmssdMillis", record.heartRateVariabilityMillis)
            is HeightRecord -> json("heightMeters", record.height.inMeters)
            is HydrationRecord -> json("volumeLiters", record.volume.inLiters)
            is IntermenstrualBleedingRecord -> JSONObject()
            is LeanBodyMassRecord -> json("massGrams", record.mass.inGrams)
            is MenstruationFlowRecord -> json("flow", record.flow.toString())
            is MenstruationPeriodRecord -> JSONObject()
            is NutritionRecord -> nutritionValues(record)
            is OvulationTestRecord -> json("result", record.result.toString())
            is OxygenSaturationRecord -> json("percentage", record.percentage.value)
            is PowerRecord -> json("sampleCount", record.samples.size)
            is RespiratoryRateRecord -> json("rate", record.rate)
            is RestingHeartRateRecord -> json("beatsPerMinute", record.beatsPerMinute)
            is SexualActivityRecord -> json("protectionUsed", record.protectionUsed.toString())
            is SleepSessionRecord -> sleepValues(record)
            is SpeedRecord -> json("sampleCount", record.samples.size)
            is StepsCadenceRecord -> json("sampleCount", record.samples.size)
            is StepsRecord -> json("count", record.count)
            is TotalCaloriesBurnedRecord -> json("energyKilocalories", record.energy.inKilocalories)
            is Vo2MaxRecord -> json("millilitersPerMinuteKilogram", record.vo2MillilitersPerMinuteKilogram)
            is WeightRecord -> json("weightGrams", record.weight.inGrams)
            is WheelchairPushesRecord -> json("count", record.count)
            else -> JSONObject()
        }
    }

    private fun bloodGlucoseValues(record: BloodGlucoseRecord): JSONObject {
        return json("levelMillimolesPerLiter", record.level.inMillimolesPerLiter)
            .put("mealType", record.mealType)
            .put("relationToMeal", record.relationToMeal.toString())
            .put("specimenSource", record.specimenSource.toString())
    }

    private fun bloodPressureValues(record: BloodPressureRecord): JSONObject {
        return json("systolicMillimetersOfMercury", record.systolic.inMillimetersOfMercury)
            .put("diastolicMillimetersOfMercury", record.diastolic.inMillimetersOfMercury)
            .put("bodyPosition", record.bodyPosition.toString())
            .put("measurementLocation", record.measurementLocation.toString())
    }

    private fun exerciseValues(record: ExerciseSessionRecord): JSONObject {
        return json("exerciseType", record.exerciseType)
            .put("title", record.title ?: "")
            .put("notes", record.notes ?: "")
            .put("segmentCount", record.segments.size)
            .put("lapCount", record.laps.size)
    }

    private fun heartRateValues(record: HeartRateRecord): JSONObject {
        val samples = JSONArray()
        record.samples.forEach { sample -> samples.put(sample.beatsPerMinute) }
        return json("sampleCount", record.samples.size).put("beatsPerMinuteSamples", samples)
    }

    private fun nutritionValues(record: NutritionRecord): JSONObject {
        return json("energyKilocalories", record.energy?.inKilocalories ?: 0.0)
            .put("energyFromFatKilocalories", record.energyFromFat?.inKilocalories ?: 0.0)
            .put("proteinGrams", record.protein?.inGrams ?: 0.0)
            .put("totalFatGrams", record.totalFat?.inGrams ?: 0.0)
            .put("totalCarbohydrateGrams", record.totalCarbohydrate?.inGrams ?: 0.0)
            .put("sugarGrams", record.sugar?.inGrams ?: 0.0)
            .put("sodiumMilligrams", record.sodium?.inMilligrams ?: 0.0)
            .put("mealType", record.mealType)
            .put("name", record.name ?: "")
    }

    private fun sleepValues(record: SleepSessionRecord): JSONObject {
        return json("title", record.title ?: "")
            .put("notes", record.notes ?: "")
            .put("stageCount", record.stages.size)
    }

    private fun json(name: String, value: Any): JSONObject {
        val json = JSONObject()
        json.put(name, value)
        return json
    }
}
