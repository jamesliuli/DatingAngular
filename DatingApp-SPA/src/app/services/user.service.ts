import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AuthService } from './auth.service';
import { map } from 'rxjs/operators';
import { Message } from '../_models/messages';

// const httpOptions = {
//   headers: new HttpHeaders({
//     Authorization: 'Bearer ' + localStorage.getItem('token')
//   })
// };

@Injectable({
  providedIn: 'root'
})
export class UserService {
// baseurl: environment.apiUrl;
baseUrl = 'http://localhost:5000/api/';

/**
 *
 */
  constructor(private http: HttpClient) {
    }

  getUsers(): Observable<User[]> {
    console.log(this.baseUrl + 'users');
    //return this.http.get<User[]>(this.baseUrl + 'users', httpOptions);
    return this.http.get<User[]>(this.baseUrl + 'users');
  }

  getUser(id: number): Observable<User> {
    //return this.http.get<User>(this.baseUrl + 'users/' + id, httpOptions);
    return this.http.get<User>(this.baseUrl + 'users/' + id);
  }

  updateUser(id: number, user: User) {
    return this.http.put(this.baseUrl + 'users/' + id, user);
  }

  deletePhoto(userId: number, photoId: number) {
    return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + photoId);
  }

  setMainPhoto(userId: number, photoId: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/photos/' + photoId + '/setMain', null);
  }

  sendLikeUser(userId: number, reciptentId: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/like/' + reciptentId, null);
  }

  getLikers(userId: number, liker: boolean): Observable<User[]> {
    const url = this.baseUrl + 'users/' + userId + '/liker?Liker=' + liker;
    console.log(url);
    return this.http.get<User[]>(url);
  }

  getMessages(id: number, messageContainer?: string) {
    let params = new HttpParams();
    params = params.append('userId', id.toString());
    params = params.append('MessageContainer', messageContainer);

    return this.http.get<Message[]>(this.baseUrl + 'users/' + id + '/messages', {observe: 'response', params})
    .pipe(
      map(response => {
        return response.body;
      }));
  }

  getMessageThread(id: number, recipientId: number) {
    return this.http.get<Message[]>(this.baseUrl + 'users/' + id + '/messages/thread/' + recipientId);
  }
}
