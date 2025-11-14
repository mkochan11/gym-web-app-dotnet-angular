import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { HttpService } from './http.service';
import { jwtDecode } from 'jwt-decode';

const TOKEN_KEY = 'auth_token';
const ROLE_KEY = 'user_role';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  constructor(private httpService: HttpService) {}

  login(credentials: { email: string; password: string }): Observable<any> {
    return this.httpService.post<any>('Auth/login', credentials)
      .pipe(
        tap(resp => {
          const tokenString = resp?.token?.result;
          if (tokenString){
            this.setToken(tokenString);
            this.cacheRoleFromToken();
          }
        })
      );
  }

  setToken(token: string) {
    try {
      localStorage.setItem(TOKEN_KEY, token);
    } catch {}
  }

  cacheRoleFromToken() {
    const token = this.getToken();
    if (!token) return;

    try {
      const decoded: any = jwtDecode(token);

      const roleClaim = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

      const role = decoded[roleClaim];
      if (role) {
        localStorage.setItem(ROLE_KEY, role);
      }
    } catch (err) {
      console.error("Failed to decode token", err);
    }
  }

  getToken(): string | null {
    try {
      return localStorage.getItem(TOKEN_KEY);
    } catch {
      return null;
    }
  }

  getRole(): string | null {
    return localStorage.getItem(ROLE_KEY);
  }

  logout(): void {
    try {
      localStorage.removeItem(TOKEN_KEY);
      localStorage.removeItem(ROLE_KEY);
    } catch {}
  }
}
