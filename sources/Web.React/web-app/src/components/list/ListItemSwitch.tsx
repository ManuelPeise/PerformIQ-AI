import React from "react";
import {
  StyledListItem,
  StyledListItemText,
} from "../styledComponents/listComponents";
import { Switch } from "@mui/material";

type SwitchProps = {
  checked: boolean;
  onChange: (checked: boolean) => void;
  disabled?: boolean;
};

interface IProps {
  primary: string;
  secondary?: string;
  switchProps: SwitchProps;
}

const ListItemSwitch: React.FC<IProps> = (props) => {
  const { primary, secondary, switchProps } = props;

  return (
    <StyledListItem divider>
      <StyledListItemText primary={primary} secondary={secondary} />
      <Switch
        checked={switchProps.checked}
        onChange={(event) => switchProps.onChange(event.target.checked)}
        disabled={switchProps.disabled}
      />
    </StyledListItem>
  );
};

export default ListItemSwitch;
