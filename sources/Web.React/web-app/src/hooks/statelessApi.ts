import React from "react";
import { privateAxiosClient } from "../lib/http/privateAxiosClient";

type RequestOptions = {
  serviceUrl: string;
  parameters?: Record<string, any>;
};

const buildQueryString = (parameters: Record<string, any>): string => {
  const queryString = Object.entries(parameters)
    .map(
      ([key, value]) =>
        `${encodeURIComponent(key)}=${encodeURIComponent(value)}`,
    )
    .join("&");
  return queryString ? `?${queryString}` : "";
};

export const createStatelessApi = <TResponse, TRequest>() => {
  const baseUrl = import.meta.env.VITE_API_URL as string;

  const updateJwtToken = React.useCallback((newToken: string | null) => {
    if (newToken) {
      privateAxiosClient.defaults.headers.common["Authorization"] =
        `Bearer ${newToken}`;
    }
  }, []);

  const sendGetRequest = async (
    options: RequestOptions,
  ): Promise<TResponse> => {
    let requestUrl = `${baseUrl}${options.serviceUrl}`;

    const queryString = options.parameters
      ? buildQueryString(options.parameters)
      : null;

    if (queryString) {
      requestUrl += queryString;
    }

    const response = await privateAxiosClient.get(requestUrl, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });
    return response.data;
  };

  const sendPostRequest = async (
    options: RequestOptions,
    data: TRequest,
  ): Promise<TResponse> => {
    const requestUrl = `${baseUrl}${options.serviceUrl}`;

    const response = await privateAxiosClient.post(requestUrl, data, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
    });
    return response.data;
  };

  return {
    sendGetRequest,
    sendPostRequest,
    updateJwtToken,
  };
};
