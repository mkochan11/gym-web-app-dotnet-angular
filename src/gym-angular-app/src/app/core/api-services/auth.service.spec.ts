import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthService]
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    localStorage.clear();
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('login', () => {
    it('should call login API and store token on success', () => {
      const mockResponse = {
        token: { result: 'mock-jwt-token' }
      };

      service.login({ email: 'test@example.com', password: 'password123' }).subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/auth/login');
      expect(req.request.method).toBe('POST');
      req.flush(mockResponse);

      expect(localStorage.getItem('auth_token')).toBe('mock-jwt-token');
    });

    it('should not store token if response has no token', () => {
      service.login({ email: 'test@example.com', password: 'password123' }).subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/auth/login');
      req.flush({});

      expect(localStorage.getItem('auth_token')).toBeNull();
    });
  });

  describe('register', () => {
    it('should call register API and store token on success', () => {
      const mockResponse = {
        token: { result: 'mock-jwt-token' }
      };

      service.register({
        firstName: 'John',
        lastName: 'Doe',
        email: 'test@example.com',
        password: 'password123'
      }).subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/auth/register');
      expect(req.request.method).toBe('POST');
      req.flush(mockResponse);

      expect(localStorage.getItem('auth_token')).toBe('mock-jwt-token');
    });
  });

  describe('getToken', () => {
    it('should return token from localStorage', () => {
      localStorage.setItem('auth_token', 'test-token');
      expect(service.getToken()).toBe('test-token');
    });

    it('should return null when no token exists', () => {
      expect(service.getToken()).toBeNull();
    });
  });

  describe('logout', () => {
    it('should remove token and role from localStorage', () => {
      localStorage.setItem('auth_token', 'test-token');
      localStorage.setItem('user_role', 'Admin');

      service.logout();

      expect(localStorage.getItem('auth_token')).toBeNull();
      expect(localStorage.getItem('user_role')).toBeNull();
    });
  });

  describe('getRole', () => {
    it('should return role from localStorage', () => {
      localStorage.setItem('user_role', 'Admin');
      expect(service.getRole()).toBe('Admin');
    });

    it('should return null when no role exists', () => {
      expect(service.getRole()).toBeNull();
    });
  });
});
