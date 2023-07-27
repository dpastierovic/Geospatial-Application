import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';

@Injectable()
export class ApiInterceptor implements HttpInterceptor {
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        const baseUrl = 'https://localhost:44368/';
        const apiReq = req.clone({ url: `${baseUrl}${req.url}` });
        console.log(apiReq.url);
        return next.handle(apiReq);
    }
}
