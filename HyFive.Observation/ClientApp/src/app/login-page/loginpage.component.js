"use strict";
var __esDecorate = (this && this.__esDecorate) || function (ctor, descriptorIn, decorators, contextIn, initializers, extraInitializers) {
    function accept(f) { if (f !== void 0 && typeof f !== "function") throw new TypeError("Function expected"); return f; }
    var kind = contextIn.kind, key = kind === "getter" ? "get" : kind === "setter" ? "set" : "value";
    var target = !descriptorIn && ctor ? contextIn["static"] ? ctor : ctor.prototype : null;
    var descriptor = descriptorIn || (target ? Object.getOwnPropertyDescriptor(target, contextIn.name) : {});
    var _, done = false;
    for (var i = decorators.length - 1; i >= 0; i--) {
        var context = {};
        for (var p in contextIn) context[p] = p === "access" ? {} : contextIn[p];
        for (var p in contextIn.access) context.access[p] = contextIn.access[p];
        context.addInitializer = function (f) { if (done) throw new TypeError("Cannot add initializers after decoration has completed"); extraInitializers.push(accept(f || null)); };
        var result = (0, decorators[i])(kind === "accessor" ? { get: descriptor.get, set: descriptor.set } : descriptor[key], context);
        if (kind === "accessor") {
            if (result === void 0) continue;
            if (result === null || typeof result !== "object") throw new TypeError("Object expected");
            if (_ = accept(result.get)) descriptor.get = _;
            if (_ = accept(result.set)) descriptor.set = _;
            if (_ = accept(result.init)) initializers.unshift(_);
        }
        else if (_ = accept(result)) {
            if (kind === "field") initializers.unshift(_);
            else descriptor[key] = _;
        }
    }
    if (target) Object.defineProperty(target, contextIn.name, descriptor);
    done = true;
};
var __runInitializers = (this && this.__runInitializers) || function (thisArg, initializers, value) {
    var useValue = arguments.length > 2;
    for (var i = 0; i < initializers.length; i++) {
        value = useValue ? initializers[i].call(thisArg, value) : initializers[i].call(thisArg);
    }
    return useValue ? value : void 0;
};
var __setFunctionName = (this && this.__setFunctionName) || function (f, name, prefix) {
    if (typeof name === "symbol") name = name.description ? "[".concat(name.description, "]") : "";
    return Object.defineProperty(f, "name", { configurable: true, value: prefix ? "".concat(prefix, " ", name) : name });
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.LoginPageComponent = void 0;
var core_1 = require("@angular/core");
var urls_1 = require("../constants/urls");
var LoginPageComponent = function () {
    var _classDecorators = [(0, core_1.Component)({
            selector: 'app-loginpage',
            templateUrl: './loginpage.component.html'
        })];
    var _classDescriptor;
    var _classExtraInitializers = [];
    var _classThis;
    var LoginPageComponent = _classThis = /** @class */ (function () {
        function LoginPageComponent_1(authorizationService, fourIndicationsSessionService, gloveSessionService, protectiveEquipmentSessionService, handJewelrySessionService, requestAboutUserAccessService, toastrService, clipboardService, codeWorkCacheService) {
            this.authorizationService = authorizationService;
            this.fourIndicationsSessionService = fourIndicationsSessionService;
            this.gloveSessionService = gloveSessionService;
            this.protectiveEquipmentSessionService = protectiveEquipmentSessionService;
            this.handJewelrySessionService = handJewelrySessionService;
            this.requestAboutUserAccessService = requestAboutUserAccessService;
            this.toastrService = toastrService;
            this.clipboardService = clipboardService;
            this.codeWorkCacheService = codeWorkCacheService;
            this.isLoggedIn = false;
            this.isOnline = false;
            this.Urls = urls_1.Urls;
            this.receivedUserStatusFromServer = false;
            this.selectedInstitution = null;
            this.showErrorMessage = false;
            this.showRequestIsRegistered = false;
            this.showRequestAwaitingApproval = false;
            this.showRequestRegistration = true;
            this.isShowPseudonym = false;
        }
        LoginPageComponent_1.prototype.ngOnInit = function () {
            var _this = this;
            this.authorizationService.isLoggedIn().subscribe(function (isLoggedIn) {
                _this.receivedUserStatusFromServer = true;
                _this.isLoggedIn = isLoggedIn;
                if (isLoggedIn) {
                    _this.authorizationService.getUser().subscribe(function (user) {
                        _this.user = user;
                        _this.requestAboutUserAccessService.fetchRequestSentAlready().subscribe(function (forsporsel) {
                            if (forsporsel != null) {
                                _this.requestAboutUserAccessService.getInstitution(forsporsel.institutionId).subscribe(function (institution) {
                                    if (institution != null) {
                                        _this.institution = institution;
                                        _this.showRequestAwaitingApproval = true;
                                        _this.showRequestRegistration = false;
                                    }
                                });
                            }
                        });
                        // this does an initial load of code, so that the cache is filled and you can work offline          
                        // this.codeWorkCacheService.loadCodeworks();
                    });
                    _this.requestAboutUserAccessService.getInstitutions().subscribe(function (institutions) {
                        _this.institutions = institutions;
                    });
                }
            });
        };
        LoginPageComponent_1.prototype.ngOnDestroy = function () {
            this.toastrService.clear();
        };
        LoginPageComponent_1.prototype.logout = function () {
            this.unregisterSw().then(function () {
                window.location.href = "/account/logout";
            });
        };
        LoginPageComponent_1.prototype.unregisterSw = function () {
            return navigator.serviceWorker.getRegistrations().then(function (registrations) {
                for (var _i = 0, registrations_1 = registrations; _i < registrations_1.length; _i++) {
                    var registration = registrations_1[_i];
                    registration.unregister();
                }
            }).catch(function (err) {
            });
        };
        LoginPageComponent_1.prototype.hasLocalSessionsLying = function () {
            var hasGloveSessions = this.gloveSessionService.numberOfSessions() > 0;
            var hasProtectiveEquipmentSessions = this.protectiveEquipmentSessionService.numberOfSessions() > 0;
            var hasFourIndicationsSessions = this.fourIndicationsSessionService.numberOfSessions() > 0;
            var hasHandJewelrySessions = this.handJewelrySessionService.numberOfSessions() > 0;
            return hasGloveSessions || hasProtectiveEquipmentSessions || hasFourIndicationsSessions || hasHandJewelrySessions;
        };
        LoginPageComponent_1.prototype.receivedInternetStatus = function (isOnline) {
            this.isOnline = isOnline;
        };
        LoginPageComponent_1.prototype.sendRequest = function () {
            var _this = this;
            var _a;
            if (this.selectedInstitution) {
                var newRequestAboutUserAccess = {
                    institutionId: (_a = this.selectedInstitution) === null || _a === void 0 ? void 0 : _a.id,
                    userFirstName: this.user.firstName,
                    userLastName: this.user.lastName,
                    hprNumber: this.user.hprNumber,
                    identityPseudonym: this.user.identityPseudonym
                };
                this.requestAboutUserAccessService.sendRequestAboutUserAccess(newRequestAboutUserAccess).subscribe(function (isUserCreated) {
                    if (isUserCreated) {
                        _this.showRequestIsRegistered = true;
                        _this.showRequestRegistration = false;
                    }
                    else
                        _this.showErrorMessage = true;
                }, function (error) {
                    _this.showErrorMessage = true;
                });
            }
        };
        LoginPageComponent_1.prototype.copyPseudonymClick = function () {
            var _a;
            this.clipboardService.copy((_a = this.user) === null || _a === void 0 ? void 0 : _a.identityPseudonym);
            this.toastrService.success('Pseudonym copied to clipboard and can be pasted elsewhere using Paste (CTRL+V)');
        };
        LoginPageComponent_1.prototype.showPseudonym = function () {
            this.isShowPseudonym = true;
        };
        LoginPageComponent_1.prototype.closeInfoModal = function ($event) {
            this.isShowPseudonym = $event;
        };
        return LoginPageComponent_1;
    }());
    __setFunctionName(_classThis, "LoginPageComponent");
    (function () {
        var _metadata = typeof Symbol === "function" && Symbol.metadata ? Object.create(null) : void 0;
        __esDecorate(null, _classDescriptor = { value: _classThis }, _classDecorators, { kind: "class", name: _classThis.name, metadata: _metadata }, null, _classExtraInitializers);
        LoginPageComponent = _classThis = _classDescriptor.value;
        if (_metadata) Object.defineProperty(_classThis, Symbol.metadata, { enumerable: true, configurable: true, writable: true, value: _metadata });
        __runInitializers(_classThis, _classExtraInitializers);
    })();
    return LoginPageComponent = _classThis;
}();
exports.LoginPageComponent = LoginPageComponent;
//# sourceMappingURL=loginpage.component.js.map