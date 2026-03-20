import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { UserService } from './user.service';
import { UpdateUserRequest } from '../models/user';

describe('UserService', () => {
  let service: UserService;
  let httpMock: HttpTestingController;
  const baseUrl = 'http://localhost:5000/api';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [UserService]
    });
    service = TestBed.inject(UserService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getUsers', () => {
    it('should return an array of users', () => {
      const mockUsers = [
        { id: '1', email: 'user1@example.com', firstName: 'John', lastName: 'Doe', role: 'Client', createdAt: new Date().toISOString() },
        { id: '2', email: 'user2@example.com', firstName: 'Jane', lastName: 'Smith', role: 'Trainer', createdAt: new Date().toISOString() }
      ];

      service.getUsers().subscribe((users) => {
        expect(users.length).toBe(2);
        expect(users[0].email).toBe('user1@example.com');
        expect(users[1].firstName).toBe('Jane');
      });

      const req = httpMock.expectOne(`${baseUrl}/users`);
      expect(req.request.method).toBe('GET');
      req.flush(mockUsers);
    });
  });

  describe('getUserById', () => {
    it('should return a single user by id', () => {
      const userId = 'user-123';
      const mockUser = { id: userId, email: 'john@example.com', firstName: 'John', lastName: 'Doe', role: 'Client', createdAt: new Date().toISOString() };

      service.getUserById(userId).subscribe((user) => {
        expect(user.id).toBe(userId);
        expect(user.email).toBe('john@example.com');
      });

      const req = httpMock.expectOne(`${baseUrl}/users/${userId}`);
      expect(req.request.method).toBe('GET');
      req.flush(mockUser);
    });

    it('should return null when user not found', () => {
      const userId = 'non-existing';

      service.getUserById(userId).subscribe((user) => {
        expect(user).toBeNull();
      });

      const req = httpMock.expectOne(`${baseUrl}/users/${userId}`);
      expect(req.request.method).toBe('GET');
      req.flush(null);
    });
  });

  describe('createUser', () => {
    it('should create a new user and return it', () => {
      const newUser = {
        email: 'new@example.com',
        password: 'Password123',
        firstName: 'New',
        lastName: 'User',
        role: 'Client'
      };
      const createdUser = { id: 'new-id', ...newUser, createdAt: new Date().toISOString() };

      service.createUser(newUser).subscribe((user) => {
        expect(user).toEqual(createdUser);
      });

      const req = httpMock.expectOne(`${baseUrl}/users`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(newUser);
      req.flush(createdUser);
    });
  });

  describe('updateUser', () => {
    it('should update an existing user and return updated user', () => {
      const userId = 'user-123';
      const updateData: UpdateUserRequest = {
        id: userId,
        email: 'updated@example.com',
        firstName: 'Updated',
        lastName: 'User',
        phoneNumber: '123456789',
        role: 'Trainer'
      };
      const updatedUser = { ...updateData, createdAt: new Date().toISOString() };

      service.updateUser(userId, updateData).subscribe((user) => {
        expect(user).toEqual(updatedUser);
      });

      const req = httpMock.expectOne(`${baseUrl}/users/${userId}`);
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(updateData);
      req.flush(updatedUser);
    });

    it('should handle update errors', () => {
      const userId = 'user-123';
      const updateData: UpdateUserRequest = {
        id: userId,
        email: 'duplicate@example.com',
        firstName: 'Test',
        lastName: 'User',
        role: 'Client'
      };

      service.updateUser(userId, updateData).subscribe({
        error: (error) => {
          expect(error).toBeTruthy();
        }
      });

      const req = httpMock.expectOne(`${baseUrl}/users/${userId}`);
      expect(req.request.method).toBe('PUT');
      req.flush({ message: 'Email already exists' }, { status: 400, statusText: 'Bad Request' });
    });
  });

  describe('getRoles', () => {
    it('should return an array of roles', () => {
      const mockRoles = ['Client', 'Trainer', 'Manager', 'Admin', 'Receptionist', 'Owner'];

      service.getRoles().subscribe((roles) => {
        expect(roles).toEqual(mockRoles);
      });

      const req = httpMock.expectOne(`${baseUrl}/users/roles`);
      expect(req.request.method).toBe('GET');
      req.flush(mockRoles);
    });
  });

  describe('deleteUser', () => {
    it('should call DELETE users/{id} API and return void', (done) => {
      const userId = 'user-123';

      service.deleteUser(userId).subscribe({
        next: (result) => {
          expect(result).toBeNull();
          done();
        },
        error: done.fail
      });

      const req = httpMock.expectOne(`${baseUrl}/users/${userId}`);
      expect(req.request.method).toBe('DELETE');
      req.flush(null);
    });

    it('should handle not found error', () => {
      const userId = 'non-existing-user';

      service.deleteUser(userId).subscribe({
        error: (error) => {
          expect(error).toBeTruthy();
        }
      });

      const req = httpMock.expectOne(`${baseUrl}/users/${userId}`);
      expect(req.request.method).toBe('DELETE');
      req.flush({ message: 'User not found' }, { status: 404, statusText: 'Not Found' });
    });

    it('should handle unauthorized error (self-delete attempt)', () => {
      const userId = 'current-user-id';

      service.deleteUser(userId).subscribe({
        error: (error) => {
          expect(error).toBeTruthy();
        }
      });

      const req = httpMock.expectOne(`${baseUrl}/users/${userId}`);
      expect(req.request.method).toBe('DELETE');
      req.flush({ errors: { DeleteUser: ['You cannot delete your own account'] } }, { status: 400, statusText: 'Bad Request' });
    });
  });
});
