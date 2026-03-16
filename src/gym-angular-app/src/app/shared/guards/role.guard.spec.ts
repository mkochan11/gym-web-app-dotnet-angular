import { TestBed } from '@angular/core/testing';
import { CanActivateFn, Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { roleGuard } from './role.guard';
import { AuthService } from '../../core/api-services';

describe('roleGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) =>
    TestBed.runInInjectionContext(() => roleGuard(...guardParameters));

  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let router: Router;
  let navigateSpy: jasmine.Spy;

  beforeEach(() => {
    localStorage.clear();
    const authServiceMock = jasmine.createSpyObj('AuthService', ['getRole']);
    authServiceMock.getRole.and.returnValue('admin');

    TestBed.configureTestingModule({
      imports: [RouterTestingModule],
      providers: [
        { provide: AuthService, useValue: authServiceMock }
      ]
    });

    authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    router = TestBed.inject(Router);
    navigateSpy = spyOn(router, 'navigate');
  });

  afterEach(() => {
    localStorage.clear();
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });

  it('should allow access when no roles required', () => {
    const result = executeGuard({ data: {} } as any, { url: '/test' } as any);
    expect(result).toBe(true);
    expect(navigateSpy).not.toHaveBeenCalled();
  });

  it('should allow access when user has required role', () => {
    authServiceSpy.getRole.and.returnValue('admin');
    const result = executeGuard({ data: { roles: ['admin'] } } as any, { url: '/test' } as any);
    expect(result).toBe(true);
  });

  it('should redirect to login when user has no role', () => {
    authServiceSpy.getRole.and.returnValue(null);
    const result = executeGuard({ data: { roles: ['admin'] } } as any, { url: '/test' } as any);
    expect(result).toBe(false);
    expect(navigateSpy).toHaveBeenCalledWith(['/login'], { queryParams: { returnUrl: '/test' } });
  });
});
