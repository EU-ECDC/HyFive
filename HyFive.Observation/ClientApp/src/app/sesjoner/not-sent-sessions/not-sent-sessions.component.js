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
exports.NotSentSessionsComponent = void 0;
var core_1 = require("@angular/core");
var urls_1 = require("../../constants/urls");
var free_solid_svg_icons_1 = require("@fortawesome/free-solid-svg-icons");
var session_type_mapper_1 = require("../../utils/session-type-mapper");
var SessionType_1 = require("../../models/api/SessionType");
var rxjs_1 = require("rxjs");
var operators_1 = require("rxjs/operators");
var NotSentSessionsComponent = function () {
    var _classDecorators = [(0, core_1.Component)({
            selector: "app-not-sent-sessions",
            templateUrl: "./not-sent-sessions.component.html",
        })];
    var _classDescriptor;
    var _classExtraInitializers = [];
    var _classThis;
    var NotSentSessionsComponent = _classThis = /** @class */ (function () {
        function NotSentSessionsComponent_1(fourIndicationsSessionService, handJewelrySessionService, gloveSessionService, protectiveEquipmentSessionService, toastrService) {
            this.fourIndicationsSessionService = fourIndicationsSessionService;
            this.handJewelrySessionService = handJewelrySessionService;
            this.gloveSessionService = gloveSessionService;
            this.protectiveEquipmentSessionService = protectiveEquipmentSessionService;
            this.toastrService = toastrService;
            this.Urls = urls_1.Urls;
            this.keyword = null;
            this.isOnline = true;
            this.hasSelectedASession = false;
            this.faCalendar = free_solid_svg_icons_1.faCalendar;
            this.sessionNameMap = session_type_mapper_1.SessionTypeMapper.getNameMap();
        }
        NotSentSessionsComponent_1.prototype.ngOnInit = function () {
            this.loadSessions();
        };
        NotSentSessionsComponent_1.prototype.ngOnDestroy = function () {
            this.toastrService.clear();
        };
        NotSentSessionsComponent_1.prototype.loadSessions = function () {
            var _this = this;
            this.sessions = this.fourIndicationsSessionService
                .getSessions()
                .map(function (f) { return _this.createSessionView(f, SessionType_1.SessionType.FourIndications); })
                .concat(this.handJewelrySessionService
                .getSessions()
                .map(function (h) { return _this.createSessionView(h, SessionType_1.SessionType.HandJewelry); }))
                .concat(this.gloveSessionService
                .getSessions()
                .map(function (h) { return _this.createSessionView(h, SessionType_1.SessionType.Gloves); }))
                .concat(this.protectiveEquipmentSessionService
                .getSessions()
                .map(function (b) { return _this.createSessionView(b, SessionType_1.SessionType.ProtectiveEquipment); }))
                .sort(function (a, b) {
                if (a.startTime > b.startTime) {
                    return -1;
                }
                if (a.startTime < b.startTime) {
                    return 1;
                }
                return 0;
            });
            this.sessionsFiltered = this.sessions;
        };
        NotSentSessionsComponent_1.prototype.filterSessions = function () {
            var _this = this;
            if (this.keyword != null && this.sessions != null) {
                this.sessionsFiltered = this.sessions.filter(function (s) {
                    var _a, _b;
                    return ((_a = s.departmentName) === null || _a === void 0 ? void 0 : _a.toLowerCase().indexOf(_this.keyword.toLowerCase())) !=
                        -1 ||
                        ((_b = _this.sessionNameMap
                            .get(s.type)) === null || _b === void 0 ? void 0 : _b.toLowerCase().indexOf(_this.keyword.toLowerCase())) != -1;
                });
            }
            else {
                this.sessionsFiltered = this.sessions;
            }
        };
        NotSentSessionsComponent_1.prototype.createSessionView = function (session, sessionType) {
            var _a;
            return {
                departmentName: (_a = session.department) === null || _a === void 0 ? void 0 : _a.name,
                startTime: session.startTime,
                type: sessionType,
                id: session.id,
                institutionsName: session.institutionsName,
            };
        };
        NotSentSessionsComponent_1.prototype.getSessionTypeUrl = function (sessionType) {
            switch (sessionType) {
                case SessionType_1.SessionType.FourIndications:
                    return urls_1.Urls.FourIndicationsSessionUrl;
                case SessionType_1.SessionType.HandJewelry:
                    return urls_1.Urls.HandJewelrySessionUrl;
                case SessionType_1.SessionType.Gloves:
                    return urls_1.Urls.GloveSessionUrl;
                case SessionType_1.SessionType.ProtectiveEquipment:
                    return urls_1.Urls.ProtectiveEquipmentSessionUrl;
                default:
                    return "";
            }
        };
        NotSentSessionsComponent_1.prototype.sendSelectedSessionsToServer = function () {
            var _this = this;
            var observables = [];
            this.sessionsFiltered.forEach(function (s) {
                if (s.isSelected) {
                    var observable = void 0;
                    switch (s.type) {
                        case SessionType_1.SessionType.FourIndications:
                            observable = _this.fourIndicationsSessionService
                                .sendToServer(s.id).pipe((0, operators_1.tap)(function () {
                                var index = _this.sessionsFiltered.findIndex(function (sf) { return sf.id === s.id; });
                                if (index > -1) {
                                    _this.sessionsFiltered.splice(index, 1);
                                }
                                _this.fourIndicationsSessionService.deleteSession(s.id);
                            }), (0, operators_1.catchError)(function (error) {
                                console.error('Error in session:', error);
                                return (0, rxjs_1.of)(null); // Return a null value so forkJoin still completes
                            }));
                            break;
                        case SessionType_1.SessionType.HandJewelry:
                            observable = _this.handJewelrySessionService
                                .sendToServer(s.id).pipe((0, operators_1.tap)(function () {
                                var index = _this.sessionsFiltered.findIndex(function (sf) { return sf.id === s.id; });
                                if (index > -1) {
                                    _this.sessionsFiltered.splice(index, 1);
                                }
                                _this.handJewelrySessionService.deleteSession(s.id);
                            }), (0, operators_1.catchError)(function (error) {
                                console.error('Error in session:', error);
                                return (0, rxjs_1.of)(null);
                            }));
                            break;
                        case SessionType_1.SessionType.Gloves:
                            observable = _this.gloveSessionService
                                .sendToServer(s.id).pipe((0, operators_1.tap)(function () {
                                var index = _this.sessionsFiltered.findIndex(function (sf) { return sf.id === s.id; });
                                if (index > -1) {
                                    _this.sessionsFiltered.splice(index, 1);
                                }
                                _this.gloveSessionService.deleteSession(s.id);
                            }), (0, operators_1.catchError)(function (error) {
                                console.error('Error in session:', error);
                                return (0, rxjs_1.of)(null);
                            }));
                            break;
                        case SessionType_1.SessionType.ProtectiveEquipment:
                            observable = _this.protectiveEquipmentSessionService
                                .sendToServer(s.id).pipe((0, operators_1.tap)(function () {
                                var index = _this.sessionsFiltered.findIndex(function (sf) { return sf.id === s.id; });
                                if (index > -1) {
                                    _this.sessionsFiltered.splice(index, 1);
                                }
                                _this.protectiveEquipmentSessionService.deleteSession(s.id);
                            }), (0, operators_1.catchError)(function (error) {
                                console.error('Error in session:', error);
                                return (0, rxjs_1.of)(null);
                            }));
                            break;
                    }
                    if (observable) {
                        observables.push(observable);
                    }
                }
            });
            (0, rxjs_1.forkJoin)(observables).subscribe({
                next: function () {
                    _this.toastrService.success("The sessions were sent to the server");
                },
                error: function (err) {
                    _this.toastrService.error("Error sending sessions to server");
                }
            });
        };
        NotSentSessionsComponent_1.prototype.markSession = function (session) {
            session.isSelected = !session.isSelected;
            this.hasSelectedASession = this.sessionsFiltered.some(function (s) { return s.isSelected; });
        };
        NotSentSessionsComponent_1.prototype.markAllSessions = function () {
            this.sessionsFiltered.forEach(function (s) { return (s.isSelected = true); });
            this.hasSelectedASession = true;
        };
        return NotSentSessionsComponent_1;
    }());
    __setFunctionName(_classThis, "NotSentSessionsComponent");
    (function () {
        var _metadata = typeof Symbol === "function" && Symbol.metadata ? Object.create(null) : void 0;
        __esDecorate(null, _classDescriptor = { value: _classThis }, _classDecorators, { kind: "class", name: _classThis.name, metadata: _metadata }, null, _classExtraInitializers);
        NotSentSessionsComponent = _classThis = _classDescriptor.value;
        if (_metadata) Object.defineProperty(_classThis, Symbol.metadata, { enumerable: true, configurable: true, writable: true, value: _metadata });
        __runInitializers(_classThis, _classExtraInitializers);
    })();
    return NotSentSessionsComponent = _classThis;
}();
exports.NotSentSessionsComponent = NotSentSessionsComponent;
//# sourceMappingURL=not-sent-sessions.component.js.map