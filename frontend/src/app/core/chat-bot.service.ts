import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

export interface ChatMessage {
  role: 'user' | 'assistant';
  content: string;
}

export interface ChatReply {
  content: string;
}

@Injectable({ providedIn: 'root' })
export class ChatBotService {
  private http = inject(HttpClient);
  private auth = inject(AuthService);

  send(messages: ChatMessage[]): Observable<ChatReply> {
    return this.http.post<ChatReply>(`${this.auth.getApiUrl()}/chat`, { messages });
  }
}
