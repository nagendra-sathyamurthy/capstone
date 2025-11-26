import React, { useState, useEffect, ReactElement } from 'react';
import { useNavigate } from 'react-router-dom';
import { ArrowLeft, Clock, CheckCircle, AlertCircle, Copy } from 'lucide-react';
import { useCart } from '../context/CartContext';
import { toast } from 'react-toastify';
import '../styles/Payment.css';

const Payment: React.FC = () => {
  const navigate = useNavigate();
  const { orderSummary, clearCart, saveCompletedOrder } = useCart();
  const [paymentStatus, setPaymentStatus] = useState<string>('pending'); // pending, processing, success, failed
  const [timeLeft, setTimeLeft] = useState<number>(15 * 60); // 15 minutes in seconds
  const [qrCode, setQrCode] = useState<string>('');

  useEffect(() => {
    if (!orderSummary) {
      navigate('/checkout');
      return;
    }

    // Generate QR code data
    const upiString = `upi://pay?pa=restaurant@upi&pn=FoodApp&am=${orderSummary.finalTotal}&tn=Order${orderSummary.orderId}&cu=INR`;
    setQrCode(upiString);

    // Start countdown timer
    const timer = setInterval(() => {
      setTimeLeft((prev) => {
        if (prev <= 1) {
          clearInterval(timer);
          setPaymentStatus('failed');
          toast.error('Payment session expired');
          return 0;
        }
        return prev - 1;
      });
    }, 1000);

    return () => clearInterval(timer);
  }, [orderSummary, navigate]);

  const formatTime = (seconds: number): string => {
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
  };

  const handleCopyUPI = (): void => {
    navigator.clipboard.writeText('restaurant@upi');
    toast.success('UPI ID copied to clipboard!');
  };

  const simulatePaymentVerification = (): void => {
    setPaymentStatus('processing');
    toast.info('Verifying payment...');

    // Simulate payment processing
    setTimeout(() => {
      setPaymentStatus('success');
      toast.success('Payment successful! Order placed.');
      
      // Save order to history
      if (orderSummary && saveCompletedOrder) {
        saveCompletedOrder(orderSummary);
      }
      
      // Clear cart
      clearCart();
      
      // Redirect to dashboard immediately after success
      setTimeout(() => {
        navigate('/dashboard', { replace: true });
      }, 2000);
    }, 2000);
  };

  const generateQRCodeSVG = (data: string): ReactElement => {
    // Simple QR code placeholder - in a real app, use a proper QR code library
    return (
      <div className="qr-placeholder">
        <div className="qr-grid">
          {Array.from({ length: 21 }, (_, i) => (
            <div key={i} className="qr-row">
              {Array.from({ length: 21 }, (_, j) => (
                <div
                  key={j}
                  className={`qr-dot ${(i + j) % 3 === 0 ? 'filled' : ''}`}
                />
              ))}
            </div>
          ))}
        </div>
        <p className="qr-text">QR Code</p>
        <p className="scan-text">Scan to Pay ₹{orderSummary?.finalTotal}</p>
      </div>
    );
  };

  if (!orderSummary) {
    return <div>Loading...</div>;
  }

  return (
    <div className="payment-container">
      <header className="payment-header">
        <button
          className="back-button"
          onClick={() => navigate('/checkout')}
          disabled={paymentStatus === 'processing'}
        >
          <ArrowLeft size={20} />
        </button>
        <h1>Payment</h1>
        <div className="timer">
          <Clock size={16} />
          {formatTime(timeLeft)}
        </div>
      </header>

      <div className="payment-content">
        {paymentStatus === 'success' ? (
          <div className="payment-success">
            <CheckCircle size={80} className="success-icon" />
            <h2>Payment Successful!</h2>
            <p>Order ID: {orderSummary.orderId}</p>
            <p>Amount Paid: ₹{orderSummary.finalTotal}</p>
            <p>Estimated Delivery: {orderSummary.estimatedDeliveryTime}</p>
            <div className="success-message">
              <p>🎉 Your order has been placed successfully!</p>
              <p>You will receive SMS updates about your order.</p>
            </div>
          </div>
        ) : paymentStatus === 'failed' ? (
          <div className="payment-failed">
            <AlertCircle size={80} className="error-icon" />
            <h2>Payment Failed</h2>
            <p>The payment session has expired or failed.</p>
            <button
              className="retry-button"
              onClick={() => navigate('/checkout')}
            >
              Try Again
            </button>
          </div>
        ) : (
          <>
            {/* Order Summary */}
            <div className="order-summary-card">
              <h2>Order Summary</h2>
              <div className="order-info">
                <div className="info-row">
                  <span>Order ID:</span>
                  <span>{orderSummary.orderId}</span>
                </div>
                <div className="info-row">
                  <span>Items:</span>
                  <span>{orderSummary.items.length} item{orderSummary.items.length > 1 ? 's' : ''}</span>
                </div>
                <div className="info-row">
                  <span>Delivery Address:</span>
                  <span className="address-text">{orderSummary.deliveryAddress.address}</span>
                </div>
                <div className="info-row total">
                  <span>Total Amount:</span>
                  <span>₹{orderSummary.finalTotal}</span>
                </div>
              </div>
            </div>

            {/* Payment Methods */}
            <div className="payment-methods">
              <h2>Choose Payment Method</h2>
              
              {/* UPI Payment */}
              <div className="payment-method-card active">
                <h3>🏦 UPI Payment</h3>
                
                <div className="qr-section">
                  <div className="qr-code">
                    {generateQRCodeSVG(qrCode)}
                  </div>
                  
                  <div className="payment-instructions">
                    <h4>How to pay:</h4>
                    <ol>
                      <li>Open any UPI app (GPay, PhonePe, Paytm, etc.)</li>
                      <li>Scan the QR code above</li>
                      <li>Verify the amount ₹{orderSummary.finalTotal}</li>
                      <li>Complete the payment</li>
                    </ol>
                  </div>
                </div>

                {/* Manual UPI Option */}
                <div className="manual-upi">
                  <h4>Or pay manually:</h4>
                  <div className="upi-details">
                    <div className="upi-id">
                      <span>UPI ID: restaurant@upi</span>
                      <button
                        className="copy-button"
                        onClick={handleCopyUPI}
                      >
                        <Copy size={16} />
                      </button>
                    </div>
                    <div className="amount">Amount: ₹{orderSummary.finalTotal}</div>
                  </div>
                </div>

                {/* Payment Status */}
                {paymentStatus === 'processing' ? (
                  <div className="payment-processing">
                    <div className="loading-spinner"></div>
                    <p>Verifying payment...</p>
                  </div>
                ) : (
                  <button
                    className="verify-payment-button"
                    onClick={simulatePaymentVerification}
                  >
                    I have completed the payment
                  </button>
                )}
              </div>

              {/* Other Payment Methods (Disabled for demo) */}
              <div className="payment-method-card disabled">
                <h3>💳 Credit/Debit Card</h3>
                <p>Coming soon</p>
              </div>

              <div className="payment-method-card disabled">
                <h3>💰 Cash on Delivery</h3>
                <p>Not available for online orders</p>
              </div>
            </div>

            {/* Security Notice */}
            <div className="security-notice">
              <h4>🔒 Secure Payment</h4>
              <p>Your payment is secured with bank-grade encryption. We never store your payment details.</p>
            </div>
          </>
        )}
      </div>
    </div>
  );
};

export default Payment;