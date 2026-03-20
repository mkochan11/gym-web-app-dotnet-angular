import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { UsersComponent } from './users.component';
import { UserService } from '../../../core/api-services/user.service';
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
import { TooltipModule } from 'primeng/tooltip';
import { of, throwError } from 'rxjs';

describe('UsersComponent', () => {
  let component: UsersComponent;
  let fixture: ComponentFixture<UsersComponent>;
  let userService: jasmine.SpyObj<UserService>;

  const mockUsers = [
    { id: '1', email: 'user1@example.com', firstName: 'John', lastName: 'Doe', role: 'Client', createdAt: new Date().toISOString() },
    { id: '2', email: 'user2@example.com', firstName: 'Jane', lastName: 'Smith', role: 'Trainer', createdAt: new Date().toISOString() }
  ];

  const mockRoles = ['Client', 'Trainer', 'Manager', 'Admin', 'Receptionist', 'Owner'];

  beforeEach(async () => {
    const userServiceSpy = jasmine.createSpyObj('UserService', ['getUsers', 'getUserById', 'createUser', 'updateUser', 'getRoles']);
    userServiceSpy.getUsers.and.returnValue(of(mockUsers));
    userServiceSpy.getRoles.and.returnValue(of(mockRoles));
    userServiceSpy.createUser.and.returnValue(of(mockUsers[0]));
    userServiceSpy.updateUser.and.returnValue(of({ ...mockUsers[0], firstName: 'Updated' }));

    await TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
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
        UsersComponent
      ],
      providers: [
        MessageService,
        FormBuilder,
        { provide: UserService, useValue: userServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UsersComponent);
    component = fixture.componentInstance;
    userService = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('should load users on init', () => {
      component.ngOnInit();
      expect(userService.getUsers).toHaveBeenCalled();
    });

    it('should load roles on init', () => {
      component.ngOnInit();
      expect(userService.getRoles).toHaveBeenCalled();
    });

    it('should map roles to RoleOption format', () => {
      component.ngOnInit();
      expect(component.roles).toEqual(mockRoles.map(r => ({ label: r, value: r })));
    });
  });

  describe('loadUsers', () => {
    it('should load users into component', fakeAsync(() => {
      component.users = [];
      component.loadUsers();
      tick();
      expect(component.users).toEqual(mockUsers);
      expect(component.loading).toBeFalse();
    }));

    it('should set loading to false on error', fakeAsync(() => {
      userService.getUsers.and.returnValue(throwError(() => new Error('API Error')));
      component.loading = false;
      component.loadUsers();
      tick();
      expect(component.loading).toBeFalse();
    }));
  });

  describe('openNewUser', () => {
    it('should open dialog in create mode', () => {
      component.openNewUser();
      expect(component.userDialog).toBeTrue();
      expect(component.isEditMode).toBeFalse();
      expect(component.editingUserId).toBeNull();
    });

    it('should reset form', () => {
      component.openNewUser();
      expect(component.userForm.get('email')?.value).toBe('');
      expect(component.userForm.get('firstName')?.value).toBe('');
    });

    it('should set password as required', () => {
      component.openNewUser();
      const passwordErrors = component.userForm.get('password')?.errors;
      expect(passwordErrors).toBeTruthy();
      expect(passwordErrors!['required']).toBeTruthy();
    });
  });

  describe('editUser', () => {
    it('should open dialog in edit mode', () => {
      component.editUser(mockUsers[0]);
      expect(component.userDialog).toBeTrue();
      expect(component.isEditMode).toBeTrue();
      expect(component.editingUserId).toBe('1');
    });

    it('should populate form with user data', () => {
      component.editUser(mockUsers[0]);
      expect(component.userForm.get('email')?.value).toBe('user1@example.com');
      expect(component.userForm.get('firstName')?.value).toBe('John');
      expect(component.userForm.get('lastName')?.value).toBe('Doe');
      expect(component.userForm.get('role')?.value).toBe('Client');
    });

    it('should clear password validators', () => {
      component.editUser(mockUsers[0]);
      expect(component.userForm.get('password')?.errors).toBeNull();
    });
  });

  describe('hideDialog', () => {
    it('should close dialog', () => {
      component.userDialog = true;
      component.hideDialog();
      expect(component.userDialog).toBeFalse();
    });

    it('should reset edit mode', () => {
      component.isEditMode = true;
      component.editingUserId = '123';
      component.hideDialog();
      expect(component.isEditMode).toBeFalse();
      expect(component.editingUserId).toBeNull();
    });

    it('should reset submitted flag', () => {
      component.submitted = true;
      component.hideDialog();
      expect(component.submitted).toBeFalse();
    });
  });

  describe('saveUser', () => {
    it('should return early if form is invalid', fakeAsync(() => {
      component.userForm.patchValue({ email: '' });
      component.submitted = false;
      component.saveUser();
      tick();
      expect(component.submitted).toBeTrue();
      expect(userService.createUser).not.toHaveBeenCalled();
      expect(userService.updateUser).not.toHaveBeenCalled();
    }));

    it('should call createUser when not in edit mode', () => {
      component.isEditMode = false;
      component.editingUserId = null;
      component.userForm.patchValue({
        email: 'new@example.com',
        password: 'Password123',
        firstName: 'New',
        lastName: 'User',
        role: 'Client'
      });
      component.saveUser();
      expect(userService.createUser).toHaveBeenCalled();
    });

    it('should call updateUser when in edit mode', () => {
      component.isEditMode = true;
      component.editingUserId = '1';
      component.userForm.patchValue({
        email: 'updated@example.com',
        firstName: 'Updated',
        lastName: 'User',
        role: 'Trainer'
      });
      component.saveUser();
      expect(userService.updateUser).toHaveBeenCalled();
    });

    it('should add new user to list on success', fakeAsync(() => {
      const initialCount = component.users.length;
      component.isEditMode = false;
      component.editingUserId = null;
      component.userDialog = true;
      component.userForm.patchValue({
        email: 'new@example.com',
        password: 'Password123',
        firstName: 'New',
        lastName: 'User',
        role: 'Client'
      });
      component.saveUser();
      tick();
      expect(component.users.length).toBe(initialCount + 1);
    }));

    it('should close dialog on create success', fakeAsync(() => {
      component.userDialog = true;
      component.isEditMode = false;
      component.editingUserId = null;
      component.userForm.patchValue({
        email: 'new@example.com',
        password: 'Password123',
        firstName: 'New',
        lastName: 'User',
        role: 'Client'
      });
      component.saveUser();
      tick();
      expect(component.userDialog).toBeFalse();
    }));

    it('should update user in list on update success', fakeAsync(() => {
      component.users = [...mockUsers];
      component.isEditMode = true;
      component.editingUserId = '1';
      component.userForm.patchValue({
        email: 'updated@example.com',
        firstName: 'Updated',
        lastName: 'User',
        role: 'Trainer'
      });
      component.saveUser();
      tick();
      const updatedUser = component.users.find(u => u.id === '1');
      expect(updatedUser?.firstName).toBe('Updated');
    }));

    it('should close dialog on update success', fakeAsync(() => {
      component.userDialog = true;
      component.isEditMode = true;
      component.editingUserId = '1';
      component.userForm.patchValue({
        email: 'updated@example.com',
        firstName: 'Updated',
        lastName: 'User',
        role: 'Trainer'
      });
      component.saveUser();
      tick();
      expect(component.userDialog).toBeFalse();
    }));
  });

  describe('getSeverity', () => {
    it('should return correct severity for each role', () => {
      expect(component.getSeverity('Admin')).toBe('danger');
      expect(component.getSeverity('Manager')).toBe('warning');
      expect(component.getSeverity('Trainer')).toBe('info');
      expect(component.getSeverity('Receptionist')).toBe('secondary');
      expect(component.getSeverity('Client')).toBe('success');
      expect(component.getSeverity('Unknown')).toBe('secondary');
    });
  });
});
