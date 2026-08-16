import React, { type PropsWithChildren } from "react";
import StyledBox from "../styledComponents/boxes";
import { CancelButton, ConfirmButton } from "../styledComponents/buttons";

interface IFormWrapperProps extends PropsWithChildren {
  isModified: boolean;
  saveButtonText: string;
  onSave: () => void;
  cancelButtonText?: string;
  onCancel?: () => void;
}

const FormWrapper: React.FC<IFormWrapperProps> = (props) => {
  const {
    isModified,
    saveButtonText,
    onSave,
    cancelButtonText,
    onCancel,
    children,
  } = props;

  return (
    <StyledBox sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
      <StyledBox sx={{ height: "auto", flex: 1 }}>{children}</StyledBox>
      <StyledBox
        sx={{
          display: "flex",
          justifyContent: "flex-end",
          padding: 2,
          gap: 1,
          maxHeight: "80px",
        }}
      >
        {cancelButtonText && onCancel && (
          <CancelButton
            sx={{ opacity: isModified ? 1 : 0 }}
            disabled={!isModified}
            onClick={onCancel}
          >
            {cancelButtonText}
          </CancelButton>
        )}
        <ConfirmButton
          sx={{ opacity: isModified ? 1 : 0 }}
          disabled={!isModified}
          onClick={onSave}
        >
          {saveButtonText}
        </ConfirmButton>
      </StyledBox>
    </StyledBox>
  );
};

export default FormWrapper;
