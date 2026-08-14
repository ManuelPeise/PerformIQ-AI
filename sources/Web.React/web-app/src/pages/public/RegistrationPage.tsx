import React from "react";
import type { ILocalizationProps } from "../../lib/localization/withLocalization";
import StyledBox from "../../components/styledComponents/boxes";
import StyledCard from "../../components/styledComponents/cards";
import StyledCaption from "../../components/styledComponents/typography";
import StyledDivider from "../../components/styledComponents/dividers";
import {
  ConfirmButton,
  CancelButton,
} from "../../components/styledComponents/buttons";
import FormTextField from "../../components/form/FormTextField";
import type { IRegisterRequestModel } from "../../lib/types/auth/IRegisterRequestModel";
import { useForm } from "../../hooks/useForm";
import { useNavigate } from "react-router-dom";
import { useAuthenticationContext } from "../../hooks/useAuthenticationContext";

const initializationModel: IRegisterRequestModel = {
  userName: "",
  email: "",
  password: "",
};

interface IProps extends ILocalizationProps {}

const RegistrationPage: React.FC<IProps> = (props) => {
  const { getResource } = props;
  const navigate = useNavigate();
  const { onRegisterUser } = useAuthenticationContext();

  const registerForm = useForm<IRegisterRequestModel>(initializationModel);

  const { userName, email, password } = registerForm.subscribe((state) => ({
    userName: state.userName,
    email: state.email,
    password: state.password,
  }));

  const handleCancelClick = React.useCallback(() => {
    navigate("/auth/login");
  }, [navigate]);

  const handleRegisterClick = React.useCallback(async () => {
    const result = await onRegisterUser(registerForm.state);

    if (result) {
      navigate("/auth/login");
    }
  }, [onRegisterUser, navigate, registerForm.state]);

  return (
    <StyledBox
      sx={{
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        padding: 2,
      }}
    >
      <StyledCard sx={{ margin: 0, padding: 2, opacity: 0.8, minWidth: 400 }}>
        <StyledBox
          sx={{
            flexDirection: "column",
            justifyContent: "center",
            alignItems: "center",
          }}
        >
          <StyledCaption variant="h1">
            {getResource("common.labelRegister")}
          </StyledCaption>
          <StyledDivider variant="inset" />
          <FormTextField
            label={getResource("common.labelUsername")}
            value={userName}
            onChange={(value) =>
              registerForm.handleChange({
                currentModel: { ...registerForm.state, userName: value },
              })
            }
          />
          <FormTextField
            label={getResource("common.labelEmail")}
            value={email}
            onChange={(value) =>
              registerForm.handleChange({
                currentModel: { ...registerForm.state, email: value },
              })
            }
          />
          <FormTextField
            label={getResource("common.labelPassword")}
            value={password}
            onChange={(value) =>
              registerForm.handleChange({
                currentModel: { ...registerForm.state, password: value },
              })
            }
            type="password"
          />
          <StyledDivider variant="middle" />
        </StyledBox>
        <StyledBox
          sx={{
            display: "flex",
            justifyContent: "flex-end",
            alignItems: "center",
          }}
        >
          <CancelButton onClick={handleCancelClick}>
            {getResource("common.labelCancel")}
          </CancelButton>
          <ConfirmButton onClick={handleRegisterClick}>
            {getResource("common.labelRegister")}
          </ConfirmButton>
        </StyledBox>
      </StyledCard>
    </StyledBox>
  );
};

export default RegistrationPage;
