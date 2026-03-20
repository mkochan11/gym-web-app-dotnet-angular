import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import { User, CreateUserRequest, UpdateUserRequest } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private httpService = inject(HttpService);

  getUsers(): Observable<User[]> {
    return this.httpService.get<User[]>('users');
  }

  getUserById(id: string): Observable<User> {
    return this.httpService.get<User>(`users/${id}`);
  }

  createUser(user: CreateUserRequest): Observable<User> {
    return this.httpService.post<User>('users', user);
  }

  updateUser(id: string, user: UpdateUserRequest): Observable<User> {
    return this.httpService.put<User>(`users/${id}`, user);
  }

  deleteUser(id: string): Observable<void> {
    return this.httpService.delete<void>(`users/${id}`);
  }

  getRoles(): Observable<string[]> {
    return this.httpService.get<string[]>('users/roles');
  }
}
