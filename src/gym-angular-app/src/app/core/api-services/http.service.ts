import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class HttpService {
  private baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  get<T>(endpoint: string): Observable<T> {
    return this.http.get<T>(`${this.baseUrl}/${endpoint}`)
      .pipe(catchError(this.handleError));
  }

  post<T>(endpoint: string, data: any): Observable<T> {
    return this.http.post<T>(`${this.baseUrl}/${endpoint}`, data)
      .pipe(catchError(this.handleError));
  }

  put<T>(endpoint: string, data: any): Observable<T> {
    return this.http.put<T>(`${this.baseUrl}/${endpoint}`, data)
      .pipe(catchError(this.handleError));
  }

  delete<T>(endpoint: string): Observable<T> {
    return this.http.delete<T>(`${this.baseUrl}/${endpoint}`)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: any) {
    console.error('HTTP Error:', error);
    
    let errorMessage = 'An error occurred';
    
    // Check if error response has the expected structure
    if (error.error) {
      // If there are validation details, extract the first error message
      if (error.error.details && typeof error.error.details === 'object') {
        const firstKey = Object.keys(error.error.details)[0];
        if (firstKey && Array.isArray(error.error.details[firstKey])) {
          errorMessage = error.error.details[firstKey][0];
        }
      } 
      // Otherwise use the message field if available
      else if (error.error.message) {
        errorMessage = error.error.message;
      }
    }
    
    return throwError(() => ({ message: errorMessage, originalError: error }));
  }
}
