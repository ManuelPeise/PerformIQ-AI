import type { IPermission } from "../../../../lib/types/auth/ICurrentUser";

export interface IUserDataExportModel {
  userId: number;
  firstName?: string;
  lastName?: string;
  userName: string;
  email: string;
  dateOfBirthUtc?: string;
  isActive: boolean;
  isMarkedAsDeleted: boolean;
  isMarkedAdDeletedBy?: string | null;
  isMarkedAdDeletedAt?: string | null;
  roles: string[];
  permissions: IPermission[];
}
