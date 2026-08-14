const validateEmailAddress = (email: string): boolean => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
};

const validateStringLength = (value: string, minLength: number): boolean => {
  return value.trim().length >= minLength;
};

export { validateEmailAddress, validateStringLength };
