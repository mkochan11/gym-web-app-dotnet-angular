export const ROLES = {
  CLIENT: 'Client',
  ADMIN: 'Admin',
  MANAGER: 'Manager',
  TRAINER: 'Trainer',
  RECEPTIONIST: 'Receptionist',
  OWNER: 'Owner'
} as const;

export type Role = typeof ROLES[keyof typeof ROLES];

export const MANAGEMENT_ROLES: Role[] = [
  ROLES.ADMIN,
  ROLES.MANAGER,
  ROLES.TRAINER,
  ROLES.RECEPTIONIST,
  ROLES.OWNER
];

export const ALL_ROLES: Role[] = Object.values(ROLES);
