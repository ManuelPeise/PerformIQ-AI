import { createStatelessApi } from "./statelessApi";
import useComponentInitializationAsync from "./componentInitializationAsync";
import { createGlobalState } from "./globalState";

export const performIq = {
  createStatelessApi: createStatelessApi,
  createGlobalState: createGlobalState,
  useComponentInitializationAsync: useComponentInitializationAsync,
};
