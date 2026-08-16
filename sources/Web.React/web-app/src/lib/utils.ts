import type { OverridableComponent } from "@mui/material/OverridableComponent";
import type { IHorizontalTabItem } from "../components/layouts/HorizontalTabPage";
import type { IVerticalTabItem } from "../components/layouts/VerticalTabPage";
import type { SvgIconTypeMap } from "@mui/material";

export const utils = {
  mapToVerticalTabListItems: <TModel>(
    keySelector: (item: TModel) => string | number,
    isReadonlySelector: (item: TModel) => boolean,
    titleSelector: (item: TModel) => string,
    subtitleSelector: (item: TModel) => string,
    items?: TModel[],
  ): IVerticalTabItem[] => {
    if (!items || items.length === 0) {
      return [];
    }
    return items?.map((item) => ({
      key: keySelector(item),
      isReadonly: isReadonlySelector(item),
      title: titleSelector(item),
      subtitle: subtitleSelector ? subtitleSelector(item) : undefined,
    }));
  },
  mapToHorizontalTabListItems: <TModel>(
    keySelector: (item: TModel) => string | number,
    isReadonlySelector: (item: TModel) => boolean,
    icon?: OverridableComponent<SvgIconTypeMap<{}, "svg">> & {
      muiName: string;
    },
    items?: TModel[],
  ): IHorizontalTabItem[] => {
    if (!items || items.length === 0) {
      return [];
    }
    return items?.map((item) => ({
      key: keySelector(item),
      icon: icon,
      isReadonly: isReadonlySelector(item),
      title: keySelector(item).toString(),
    }));
  },
  groupBy<T, K extends PropertyKey>(
    items: T[],
    keySelector: (item: T) => K,
  ): Record<K, T[]> {
    return items.reduce(
      (groups, item) => {
        const key = keySelector(item);

        (groups[key] ??= []).push(item);

        return groups;
      },
      {} as Record<K, T[]>,
    );
  },
  capitaliseFirstLetter: (str: string): string => {
    if (!str) {
      return str;
    }
    const trimmedStr = str.trim();
    return trimmedStr.charAt(0).toUpperCase() + trimmedStr.slice(1);
  },
  camelCaseToTitleCase: (str: string): string => {
    if (!str) {
      return str;
    }
    const result = str.replace(/([A-Z])/g, " $1");
    return result.charAt(0).toUpperCase() + result.slice(1);
  },
};
