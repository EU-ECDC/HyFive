"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
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
exports.HandJewelrySessionService = void 0;
var uuid_1 = require("../../utils/uuid");
var localstoragepaths_1 = require("../../constants/localstoragepaths");
var base_session_service_1 = require("./base-session.service");
var http_1 = require("@angular/common/http");
var environment_1 = require("../../../environments/environment");
var core_1 = require("@angular/core");
var HandJewelrySessionService = function () {
    var _classDecorators = [(0, core_1.Injectable)({
            providedIn: 'root'
        })];
    var _classDescriptor;
    var _classExtraInitializers = [];
    var _classThis;
    var _classSuper = base_session_service_1.BaseSessionService;
    var HandJewelrySessionService = _classThis = /** @class */ (function (_super) {
        __extends(HandJewelrySessionService_1, _super);
        function HandJewelrySessionService_1(institutionService, httpClient) {
            var _this = _super.call(this, institutionService) || this;
            _this.institutionService = institutionService;
            _this.httpClient = httpClient;
            _this.sessionLocalStoragePath = localstoragepaths_1.Localstoragepaths.HandJewelrySessions;
            _this.sessionShowLocalStoragePath = localstoragepaths_1.Localstoragepaths.HandJewelrySessionViews;
            return _this;
        }
        HandJewelrySessionService_1.prototype.sendToServer = function (sessionId) {
            var sessions = this.getSessions();
            var sessionIndex = sessions.map(function (s) { return s.id; }).indexOf(sessionId);
            var sessionToSend = sessions[sessionIndex];
            return this.httpClient.post("".concat(environment_1.environment.apiBaseUrl, "/v1/handjewelry"), sessionToSend);
        };
        HandJewelrySessionService_1.prototype.createSessionView = function (rolesAsObserved, department) {
            var id = uuid_1.Uuid.generateUUID();
            var handJewelrySessionView = {
                sessionId: id,
                department: department,
                card: rolesAsObserved.map(function (r, i) { return { id: uuid_1.Uuid.generateUUID(), role: r, isActive: i == 0 }; }),
            };
            var sessionViews = this.getSessionViews();
            sessionViews.push(handJewelrySessionView);
            this.saveSessionViews(sessionViews);
            return id;
        };
        HandJewelrySessionService_1.prototype.getSessionFromServer = function (sessionId) {
            if (navigator.onLine) {
                var params = new http_1.HttpParams();
                params = params.append("sessionId", sessionId);
                return this.httpClient.get("".concat(environment_1.environment.apiBaseUrl, "/v1/session/handjewelry"), { params: params });
            }
            else {
                confirm("Not connected to the internet");
            }
        };
        return HandJewelrySessionService_1;
    }(_classSuper));
    __setFunctionName(_classThis, "HandJewelrySessionService");
    (function () {
        var _a;
        var _metadata = typeof Symbol === "function" && Symbol.metadata ? Object.create((_a = _classSuper[Symbol.metadata]) !== null && _a !== void 0 ? _a : null) : void 0;
        __esDecorate(null, _classDescriptor = { value: _classThis }, _classDecorators, { kind: "class", name: _classThis.name, metadata: _metadata }, null, _classExtraInitializers);
        HandJewelrySessionService = _classThis = _classDescriptor.value;
        if (_metadata) Object.defineProperty(_classThis, Symbol.metadata, { enumerable: true, configurable: true, writable: true, value: _metadata });
        __runInitializers(_classThis, _classExtraInitializers);
    })();
    return HandJewelrySessionService = _classThis;
}();
exports.HandJewelrySessionService = HandJewelrySessionService;
//# sourceMappingURL=hand-Jewelry-session.service.js.map