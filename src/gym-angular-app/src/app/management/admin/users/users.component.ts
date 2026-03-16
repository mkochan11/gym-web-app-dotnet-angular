import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../../../core/api-services/user.service';
import { User, CreateUserRequest } from '../../../core/models/user';
import { MessageService } from 'primeng/api';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { DialogModule } from 'primeng/dialog';
import { PasswordModule } from 'primeng/password';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';
import { TagModule } from 'primeng/tag';

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
    TagModule
  ],
  providers: [MessageService],
  templateUrl: './users.component.html',
  styleUrl: './users.component.scss'
})
export class UsersComponent implements OnInit {
  private userService = inject(UserService);
  private fb = inject(FormBuilder);
  private messageService = inject(MessageService);

  users: User[] = [];
  roles: RoleOption[] = [];
  loading = false;
  submitted = false;

  userDialog = false;
  userForm: FormGroup;

  constructor() {
    this.userForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
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

  openNewUser() {
    this.userForm.reset();
    this.submitted = false;
    this.userDialog = true;
  }

  hideDialog() {
    this.userDialog = false;
    this.submitted = false;
  }

  saveUser() {
    this.submitted = true;

    if (this.userForm.invalid) {
      return;
    }

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
}
