export interface ICurrentUser {
  userId: string;
  userName: string;
  email: string;
  roles: string[];
  permissions: IPermission[];
}

export interface IPermission {
  module: string;
  canView: boolean;
  canCreate: boolean;
  canEdit: boolean;
  canDelete: boolean;
}
