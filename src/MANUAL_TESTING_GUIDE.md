# Manual Testing Guide: Bluetooth Auto-Reconnect

## Test Environment Setup

1. Build the application:
   ```powershell
   dotnet build
   ```

2. Run the application:
   ```powershell
   dotnet run --project Idasen.SystemTray.Win11
   ```

3. Monitor logs:
   - Location: `C:\ProgramData\idasen-desk\log.txt`
   - Or use: `Get-Content C:\ProgramData\idasen-desk\log.txt -Wait -Tail 50`

## Test Scenarios

### ✅ Scenario 1: Normal Connection (Baseline)
**Steps:**
1. Ensure desk is powered on and in Bluetooth range
2. Start the application
3. Wait for auto-connect (3 second delay)

**Expected Result:**
- ✅ Connection succeeds on first attempt
- ✅ Log shows: "Bluetooth connection successful on initial attempt"
- ✅ No retry attempts logged
- ✅ Tray icon shows connected state

**Status:** [ ] Pass [ ] Fail

---

### ✅ Scenario 2: Desk Off at Startup (Retry Success)
**Steps:**
1. Turn desk off or disable Bluetooth
2. Start the application
3. Wait ~5 seconds
4. Turn desk back on
5. Wait for reconnection

**Expected Result:**
- ✅ Initial connection fails
- ✅ Log shows "Starting retry sequence with exponential backoff..."
- ✅ Multiple retry attempts logged with increasing delays
- ✅ Connection eventually succeeds
- ✅ Log shows successful attempt number
- ✅ Tray icon updates to connected

**Status:** [ ] Pass [ ] Fail

**Actual Retry Count:** _____ attempts

---

### ✅ Scenario 3: Desk Never Comes Online (Exhausted Retries)
**Steps:**
1. Turn desk off or move out of range
2. Start the application
3. Keep desk off for entire retry sequence (~30 seconds)

**Expected Result:**
- ✅ Initial connection fails
- ✅ Retry sequence starts
- ✅ All 5 retry attempts occur with proper delays:
  - Retry 1: ~1s delay
  - Retry 2: ~2s delay
  - Retry 3: ~4s delay
  - Retry 4: ~8s delay
  - Retry 5: ~16s delay
- ✅ Connection fails after all retries
- ✅ Tray icon shows disconnected state
- ✅ Error notification shown to user

**Status:** [ ] Pass [ ] Fail

**Total Time:** _____ seconds (should be ~31 seconds)

---

### ✅ Scenario 4: Intermittent Connection (Retry Then Succeed)
**Steps:**
1. Turn desk off
2. Start the application
3. Wait for 2-3 retry attempts
4. Turn desk on mid-retry

**Expected Result:**
- ✅ Initial connection fails
- ✅ Some retry attempts fail
- ✅ One retry attempt succeeds
- ✅ Remaining retries are skipped
- ✅ Connection established
- ✅ Log shows which attempt succeeded

**Status:** [ ] Pass [ ] Fail

**Succeeded on attempt:** _____

---

### ✅ Scenario 5: User Cancels During Retry
**Steps:**
1. Turn desk off
2. Start the application
3. During retry sequence, close the application

**Expected Result:**
- ✅ Retry sequence stops immediately
- ✅ No additional connection attempts after cancellation
- ✅ Log shows "Retry sequence cancelled by user"
- ✅ Application exits gracefully

**Status:** [ ] Pass [ ] Fail

---

### ✅ Scenario 6: Reconnection After Disconnect
**Steps:**
1. Connect to desk normally
2. Disconnect (via tray menu or turn desk off)
3. Click "Connect" in tray menu

**Expected Result:**
- ✅ If desk available: connects immediately
- ✅ If desk not available: retry sequence starts
- ✅ Same retry behavior as startup

**Status:** [ ] Pass [ ] Fail

---

### ✅ Scenario 7: Multiple Rapid Connection Attempts
**Steps:**
1. Turn desk off
2. Start application (triggers first retry sequence)
3. Quickly click "Connect" multiple times in tray menu

**Expected Result:**
- ✅ Only one retry sequence active at a time
- ✅ Previous attempts are cancelled when new one starts
- ✅ No resource leaks or hanging connections
- ✅ Application remains responsive

**Status:** [ ] Pass [ ] Fail

---

## Performance Observations

### Timing Validation
Record actual retry delays:

| Retry # | Expected Delay | Actual Delay | ✓/✗ |
|---------|---------------|--------------|-----|
| 1       | ~1 second     | _____ sec    |     |
| 2       | ~2 seconds    | _____ sec    |     |
| 3       | ~4 seconds    | _____ sec    |     |
| 4       | ~8 seconds    | _____ sec    |     |
| 5       | ~16 seconds   | _____ sec    |     |

### Log Output Quality
- [ ] Retry attempts are clearly logged
- [ ] Delay durations are shown
- [ ] Success/failure is obvious
- [ ] No excessive/redundant logs
- [ ] Error messages are helpful

### User Experience
- [ ] Tray icon state reflects connection status
- [ ] Notifications are timely and clear
- [ ] Application remains responsive during retries
- [ ] User can cancel retry sequence
- [ ] No unexpected errors or crashes

---

## Bluetooth-Specific Tests

### Test with Bluetooth Stack Issues
**Scenario:** Test with various Bluetooth conditions

1. **Weak Signal:** Move desk to edge of Bluetooth range
   - Status: [ ] Pass [ ] Fail
   - Notes: _________________________________

2. **Multiple Bluetooth Devices:** Have many BT devices active
   - Status: [ ] Pass [ ] Fail
   - Notes: _________________________________

3. **Bluetooth Restart:** Disable/re-enable Bluetooth adapter during retry
   - Status: [ ] Pass [ ] Fail
   - Notes: _________________________________

---

## Regression Testing

### Ensure Existing Features Still Work
- [ ] Manual connect/disconnect via tray menu
- [ ] Hotkey shortcuts for sit/stand/custom positions
- [ ] Settings UI (height configuration, hotkeys, theme)
- [ ] Move commands work when connected
- [ ] Stop/lock/unlock functionality
- [ ] Application startup and shutdown

---

## Issue Tracking

### Bugs Found
| Issue # | Description | Severity | Reproducible |
|---------|-------------|----------|--------------|
|         |             |          |              |

### Observations/Improvements
| Observation | Suggested Improvement |
|-------------|----------------------|
|             |                      |

---

## Sign-off

**Tester:** _____________________  
**Date:** _____________________  
**Build/Commit:** f35f735  
**Overall Result:** [ ] Pass [ ] Fail  

**Notes:**
_________________________________________________________________
_________________________________________________________________
_________________________________________________________________
