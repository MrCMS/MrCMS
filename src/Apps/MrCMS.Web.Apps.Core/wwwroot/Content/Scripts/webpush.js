/*
*
*  Push Notifications codelab
*  Copyright 2015 Google Inc. All rights reserved.
*
*  Licensed under the Apache License, Version 2.0 (the "License");
*  you may not use this file except in compliance with the License.
*  You may obtain a copy of the License at
*
*      https://www.apache.org/licenses/LICENSE-2.0
*
*  Unless required by applicable law or agreed to in writing, software
*  distributed under the License is distributed on an "AS IS" BASIS,
*  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*  See the License for the specific language governing permissions and
*  limitations under the License
*
*/

/* eslint-env browser, es6 */

'use strict';

let applicationServerPublicKey;
let pushButton;

export function setupPushButton() {
    pushButton = document.querySelector('[data-push-notification-btn]');
    if (!pushButton) {
        return;
    }
    applicationServerPublicKey = pushButton.dataset.publicKey;

    if ('serviceWorker' in navigator && 'PushManager' in window) {
        log('Service Worker and Push is supported');

        navigator.serviceWorker.register(new URL('/sw.js'))
            .then(function (swReg) {
                log('Service Worker is registered', swReg);
                swRegistration = swReg;
                initializeUI();
            })
            .catch(function (err) {
                error('Service Worker Error', err);
            });
    } else {
        warn('Push messaging is not supported');
        pushButton.remove();
    }
}

let isSubscribed = false;
let swRegistration = null;

function urlB64ToUint8Array(base64String) {
    const padding = '='.repeat((4 - base64String.length % 4) % 4);
    const base64 = (base64String + padding)
        .replace(/\-/g, '+')
        .replace(/_/g, '/');

    const rawData = window.atob(base64);
    const outputArray = new Uint8Array(rawData.length);

    for (let i = 0; i < rawData.length; ++i) {
        outputArray[i] = rawData.charCodeAt(i);
    }
    return outputArray;
}

function updateBtn() {
    if (Notification.permission === 'denied') {
        pushButton.textContent = 'Push Messaging Blocked.';
        pushButton.disabled = true;
        return;
    }

    if (isSubscribed) {
        pushButton.textContent = pushButton.dataset.disableText ?? 'Disable notifications';
    } else {
        pushButton.textContent = pushButton.dataset.enableText ?? 'Enable notifications';
    }

    pushButton.disabled = false;
}

function subscribeOnServer(subscription) {
    if(!subscription)
        return Promise.resolve();
    const rawKey = subscription.getKey ? subscription.getKey('p256dh') : '';
    const key = rawKey ? btoa(String.fromCharCode.apply(null, new Uint8Array(rawKey))) : '';
    const rawAuthSecret = subscription.getKey ? subscription.getKey('auth') : '';
    const authSecret = rawAuthSecret ? btoa(String.fromCharCode.apply(null, new Uint8Array(rawAuthSecret))) : '';
    const endpoint = subscription.endpoint;

    return fetch('/push-notifications',
        {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': $('body').data('antiforgery-token')
            },
            body: JSON.stringify({key: key, endpoint: endpoint, authSecret: authSecret})
        });
}

function unsubscribeOnServer(endpoint) {
    return fetch('/push-notifications?endpoint=' + encodeURIComponent(endpoint),
        {
            method: 'DELETE',
            headers: {
                'RequestVerificationToken': $('body').data('antiforgery-token')
            }
        }
    );
}

function subscribeUser() {
    const applicationServerKey = urlB64ToUint8Array(applicationServerPublicKey);
    swRegistration.pushManager.subscribe({
        userVisibleOnly: true,
        applicationServerKey: applicationServerKey
    })
        .then(function (subscription) {
            log('User is subscribed.');

            return subscribeOnServer(subscription);
        })
        .then(() => {
            isSubscribed = true;

            updateBtn();

        })
        .catch(function (err) {
            log('Failed to subscribe the user: ', err);
            updateBtn();
        });
}

function unsubscribeUser() {
    swRegistration.pushManager.getSubscription()
        .then(function (subscription) {
            if (subscription) {
                return subscription.unsubscribe()
                    .then(function () {
                        unsubscribeOnServer(subscription.endpoint).then(r => {

                            log('User is unsubscribed.');
                            isSubscribed = false;
                            updateBtn();
                        });
                    });
            }
        })
        .catch(function (error) {
            log('Error unsubscribing', error);
        });
}

function initializeUI() {
    pushButton.addEventListener('click', function () {
        pushButton.disabled = true;
        if (isSubscribed) {
            unsubscribeUser();
        } else {
            subscribeUser();
        }
    });

    // Set the initial subscription value
    swRegistration.pushManager.getSubscription()
        .then(function (subscription) {
            isSubscribed = !(subscription === null);

            return subscribeOnServer(subscription);
        })
        .then(() => {
            if (isSubscribed) {
                log('User IS subscribed.');
            } else {
                log('User is NOT subscribed.');
            }

            updateBtn();
        });
}

function log(...args) {
    if (window.location.origin.indexOf('localhost') > -1)
        console.log(args)
}

function warn(...args) {
    if (window.location.origin.indexOf('localhost') > -1)
        console.warn(args)
}

function error(...args) {
    if (window.location.origin.indexOf('localhost') > -1)
        console.error(args)
}

