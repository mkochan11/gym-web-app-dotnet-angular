import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../../../core/api-services/user.service';
import { AuthService } from '../../../core/api-services/auth.service';
import { User, CreateUserRequest, UpdateUserRequest } from '../../../core/models/user';
import { MessageService, ConfirmationService } from 'primeng/api';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { DialogModule } from 'primeng/dialog';
import { PasswordModule } from 'primeng/password';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { ConfirmDialogModule } from 'primeng/confirmdialog';

interface RoleOption {
  label: string;
  value: string;
}

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    DropdownModule,
    DialogModule,
    PasswordModule,
    ToastModule,
    ToolbarModule,
    TagModule,
    TooltipModule,
    ConfirmDialogModule
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './users.component.html',
  styleUrl: './users.component.scss'
})
export class UsersComponent implements OnInit {
  private userService = inject(UserService);
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);
  private messageService = inject(MessageService);

  users: User[] = [];
  roles: RoleOption[] = [];
  loading = false;
  submitted = false;
  isEditMode = false;
  editingUserId: string | null = null;

  userDialog = false;
  userForm: FormGroup;

  deleteDialog = false;
  userToDelete: User | null = null;

  constructor() {
    this.userForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.minLength(8)]],
      firstName: ['', [Validators.required, Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.maxLength(100)]],
      phoneNumber: [''],
      role: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.loadUsers();
    this.loadRoles();
  }

  loadUsers() {
    this.loading = true;
    this.userService.getUsers().subscribe({
      next: (users) => {
        this.users = users;
        this.loading = false;
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load users' });
        this.loading = false;
      }
    });
  }

  loadRoles() {
    this.userService.getRoles().subscribe({
      next: (roles) => {
        this.roles = roles.map(r => ({ label: r, value: r }));
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load roles' });
      }
    });
  }

  hideDialog() {
    this.userDialog = false;
    this.submitted = false;
    this.isEditMode = false;
    this.editingUserId = null;
  }

  openNewUser() {
    this.userForm.reset();
    this.userForm.get('password')?.setValidators([Validators.required, Validators.minLength(8)]);
    this.submitted = false;
    this.isEditMode = false;
    this.editingUserId = null;
    this.userDialog = true;
  }

  editUser(user: User) {
    this.isEditMode = true;
    this.editingUserId = user.id;
    this.userForm.patchValue({
      email: user.email,
      password: '',
      firstName: user.firstName,
      lastName: user.lastName,
      phoneNumber: user.phoneNumber || '',
      role: user.role
    });
    this.userForm.get('password')?.clearValidators();
    this.submitted = false;
    this.userDialog = true;
  }

  saveUser() {
    this.submitted = true;

    if (this.userForm.invalid) {
      return;
    }

    if (this.isEditMode && this.editingUserId) {
      this.updateUser();
    } else {
      this.createUser();
    }
  }

  private createUser() {
    const userData: CreateUserRequest = {
      email: this.userForm.value.email,
      password: this.userForm.value.password,
      firstName: this.userForm.value.firstName,
      lastName: this.userForm.value.lastName,
      phoneNumber: this.userForm.value.phoneNumber || undefined,
      role: this.userForm.value.role
    };

    this.userService.createUser(userData).subscribe({
      next: (newUser) => {
        this.users.push(newUser);
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'User created successfully' });
        this.hideDialog();
      },
      error: (err) => {
        const errorMessage = err.error?.errors?.Email?.[0] || err.error?.errors?.CreateUser?.[0] || 'Failed to create user';
        this.messageService.add({ severity: 'error', summary: 'Error', detail: errorMessage });
      }
    });
  }

  private updateUser() {
    if (!this.editingUserId) return;

    const userData: UpdateUserRequest = {
      id: this.editingUserId,
      email: this.userForm.value.email,
      firstName: this.userForm.value.firstName,
      lastName: this.userForm.value.lastName,
      phoneNumber: this.userForm.value.phoneNumber || undefined,
      role: this.userForm.value.role
    };

    this.userService.updateUser(this.editingUserId, userData).subscribe({
      next: (updatedUser) => {
        const index = this.users.findIndex(u => u.id === updatedUser.id);
        if (index !== -1) {
          this.users[index] = updatedUser;
        }
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'User updated successfully' });
        this.hideDialog();
      },
      error: (err) => {
        const errorMessage = err.error?.errors?.Email?.[0] || err.error?.errors?.UpdateUser?.[0] || 'Failed to update user';
        this.messageService.add({ severity: 'error', summary: 'Error', detail: errorMessage });
      }
    });
  }

  getSeverity(role: string): 'success' | 'info' | 'warning' | 'danger' | 'secondary' | 'contrast' | undefined {
    switch (role) {
      case 'Admin':
        return 'danger';
      case 'Manager':
        return 'warning';
      case 'Trainer':
        return 'info';
      case 'Receptionist':
        return 'secondary';
      case 'Client':
        return 'success';
      default:
        return 'secondary';
    }
  }

  confirmDeleteUser(user: User): void {
    this.userToDelete = user;
    this.deleteDialog = true;
  }

  hideDeleteDialog(): void {
    this.deleteDialog = false;
    this.userToDelete = null;
  }

  deleteUser(): void {
    if (!this.userToDelete) return;

    this.userService.deleteUser(this.userToDelete.id).subscribe({
      next: () => {
        this.users = this.users.filter(u => u.id !== this.userToDelete!.id);
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'User deleted successfully' });
        this.hideDeleteDialog();
      },
      error: (err) => {
        const errorMessage = err.error?.errors?.DeleteUser?.[0] || 'Failed to delete user';
        this.messageService.add({ severity: 'error', summary: 'Error', detail: errorMessage });
        this.hideDeleteDialog();
      }
    });
  }

  getCurrentUserId(): string | null {
    return this.authService.getUserId();
  }
}
