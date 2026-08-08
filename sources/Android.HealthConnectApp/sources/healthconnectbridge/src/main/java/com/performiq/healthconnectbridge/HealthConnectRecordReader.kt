package com.performiq.healthconnectbridge

import android.content.Context
import androidx.health.connect.client.HealthConnectClient
import androidx.health.connect.client.records.Record
import androidx.health.connect.client.request.ReadRecordsRequest
import androidx.health.connect.client.time.TimeRangeFilter
import java.time.Instant
import kotlin.reflect.KClass

internal object HealthConnectRecordReader {
    suspend fun readAllRecords(
        context: Context,
        recordTypes: Set<KClass<out Record>>,
        startTime: Instant,
        endTime: Instant
    ): List<Record> {
        val client = HealthConnectClient.getOrCreate(context)
        val timeRangeFilter = TimeRangeFilter.between(startTime, endTime)
        val records = mutableListOf<Record>()
        for (recordType in recordTypes) {
            records.addAll(readByType(client, timeRangeFilter, recordType))
        }

        return records
    }

    private suspend fun readByType(
        client: HealthConnectClient,
        timeRangeFilter: TimeRangeFilter,
        recordType: KClass<out Record>
    ): List<Record> {
        val records = mutableListOf<Record>()
        var pageToken: String? = null

        do {
            val request = ReadRecordsRequest(
                recordType = recordType,
                timeRangeFilter = timeRangeFilter,
                pageToken = pageToken
            )
            val response = client.readRecords(request)
            records.addAll(response.records)
            pageToken = response.pageToken
        } while (!pageToken.isNullOrBlank())

        return records
    }
}
