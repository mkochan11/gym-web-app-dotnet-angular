import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import { User, CreateUserRequest } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private httpService = inject(HttpService);

  getUsers(): Observable<User[]> {
    return this.httpService.get<User[]>('users');
  }

  createUser(user: CreateUserRequest): Observable<User> {
    return this.httpService.post<User>('users', user);
  }

  getRoles(): Observable<string[]> {
    return this.httpService.get<string[]>('users/roles');
  }
}
